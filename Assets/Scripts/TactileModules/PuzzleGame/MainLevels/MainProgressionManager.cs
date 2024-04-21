using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using UnityEngine;

namespace TactileModules.PuzzleGame.MainLevels
{
	public class MainProgressionManager : IMainProgression
	{
		private MainProgressionManager(GateManager gateManager, MainProgressionManager.IDataProvider dataProvider, LevelDatabaseCollection levelDatabaseCollection, int progressionGapThreshold = 2147483647)
		{
			this.levelDatabaseCollection = levelDatabaseCollection;
			this.progressionGapThreshold = progressionGapThreshold;
			this.dataProvider = dataProvider;
			this.gateManager = gateManager;
			this.gateManager.GateUnlocked += this.HandleGateUnlocked;
			this.RetroactivelyAssignHardStars();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action DeveloperCheated;



		public static MainProgressionManager Instance { get; private set; }

		public static MainProgressionManager CreateInstance(GateManager gateManager, MainProgressionManager.IDataProvider dataProvider, LevelDatabaseCollection levelDatabaseCollection, int progressionGapThreshold = 2147483647)
		{
			MainProgressionManager.Instance = new MainProgressionManager(gateManager, dataProvider, levelDatabaseCollection, int.MaxValue);
			return MainProgressionManager.Instance;
		}

		private void HandleGateUnlocked(LevelProxy obj)
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			persistableState.RefreshFarthestCompletedLevelId();
		}

		public void Developer_CompleteLevels(int upToLevelId, int wantedStars = 3, bool notifyProvider = true)
		{
			MainLevelDatabase database = this.GetDatabase();
			for (int i = 0; i < database.NumberOfAvailableLevels; i++)
			{
				LevelProxy level = database.GetLevel(i);
				if (i <= upToLevelId)
				{
					ILevelAccomplishment levelData = level.GetLevelData(true);
					levelData.Points = 100;
					if (i == upToLevelId)
					{
						levelData.Stars = wantedStars;
						if (level.LevelDifficulty == LevelDifficulty.Hard)
						{
							this.SetHardStars(level, wantedStars);
						}
					}
					else
					{
						levelData.Stars = 3;
						if (level.LevelDifficulty == LevelDifficulty.Hard)
						{
							this.SetHardStars(level, 3);
						}
					}
				}
				else
				{
					this.SetHardStars(level, 0);
					this.RemoveLevelData(level);
				}
			}
			database.Save();
			if (notifyProvider)
			{
				this.dataProvider.HandleDeveloperCheated();
			}
			this.DeveloperCheated();
		}

		public void RegisterLevelAmountProvider(ILevelAmountProvider levelAmountProvider)
		{
			this.levelAmountProvider = levelAmountProvider;
		}

		public string GetPersistedKey(LevelProxy levelProxy)
		{
			return this.dataProvider.GetPersistedKeyForLevelProxy(levelProxy);
		}

		public ILevelAccomplishment GetLevelData(bool createIfNotExisting, LevelProxy levelProxy)
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			string persistedKey = this.GetPersistedKey(levelProxy);
			MainProgressionManager.PersistedLevelContextData persistedLevelContextData;
			if (!persistableState.Levels.TryGetValue(persistedKey, out persistedLevelContextData) && createIfNotExisting)
			{
				persistedLevelContextData = new MainProgressionManager.PersistedLevelContextData();
				persistableState.Levels.Add(persistedKey, persistedLevelContextData);
			}
			return persistedLevelContextData;
		}

		public void RemoveLevelData(LevelProxy levelProxy)
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			string persistedKey = this.GetPersistedKey(levelProxy);
			if (persistableState.Levels.ContainsKey(persistedKey))
			{
				persistableState.Levels.Remove(persistedKey);
			}
			if (persistableState.HardStars.ContainsKey(persistedKey))
			{
				persistableState.HardStars.Remove(persistedKey);
			}
		}

		public double GetGateProgress(LevelProxy playableLevel)
		{
			LevelProxy levelProxy = playableLevel.CreateParentProxy();
			int num = 0;
			if (levelProxy != LevelProxy.Invalid && levelProxy.LevelMetaData is GateMetaData)
			{
				num = this.ClampedGateProgress();
			}
			return 0.1 * (double)num;
		}

		public void GetUserProgressValues(out int highestLevel, out int gateLevel)
		{
			highestLevel = this.GetFarthestUnlockedLevelHumanNumber();
			gateLevel = 0;
			if (this.gateManager.PlayerOnGate)
			{
				gateLevel = this.ClampedGateProgress();
			}
		}

		private int ClampedGateProgress()
		{
			return Mathf.Clamp(this.gateManager.CurrentGateKeys + 1, 1, 3);
		}

		public void SaveLevelAccomplishments(LevelProxy levelToPlay, int score)
		{
			levelToPlay.SaveSessionAccomplishment(score, false);
			if (levelToPlay.LevelDifficulty == LevelDifficulty.Hard)
			{
				int stars = levelToPlay.NumberOfStarsFromPoints(score);
				this.SetHardStars(levelToPlay, stars);
			}
			this.gateManager.UpdateGate();
		}

		public int GetNumberOfStarsInLevels(int minLevelNr, int maxLevelNr)
		{
			MainLevelDatabase database = this.GetDatabase();
			int num = 0;
			for (int i = minLevelNr; i <= maxLevelNr; i++)
			{
				num += database.GetLevel(i).Stars;
			}
			return num;
		}

		public int GetNumberOfStarsInLevel(int levelIndex)
		{
			MainLevelDatabase database = this.GetDatabase();
			return database.GetLevel(levelIndex).Stars;
		}

		public int GetTotalEarnedStars()
		{
			MainLevelDatabase database = this.GetDatabase();
			int num = 0;
			for (int i = 0; i < database.NumberOfAvailableLevels; i++)
			{
				LevelProxy level = database.GetLevel(i);
				if (level.IsUnlocked)
				{
					num += level.Stars;
				}
			}
			return num;
		}

		public void Save()
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			persistableState.RefreshFarthestCompletedLevelId();
			this.dataProvider.SetPublicFarthestLevelId(persistableState.FarthestCompletedLevelId);
			this.dataProvider.SaveUserSettings();
		}

		public LevelProxy GetFarthestCompletedLevelProxy()
		{
			MainLevelDatabase database = this.GetDatabase();
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			return database.GetLevel(persistableState.FarthestCompletedLevelId);
		}

		public int GetFarthestCompletedLevelIndex()
		{
			return this.GetFarthestCompletedLevelProxy().Index;
		}

		public int GetFarthestUnlockedLevelIndex()
		{
			return Math.Max(0, this.GetFarthestUnlockedLevelProxy().Index);
		}

		public int GetFarthestCompletedLevelHumanNumber()
		{
			MainLevelDatabase database = this.GetDatabase();
			int farthestCompletedLevelIndex = this.GetFarthestCompletedLevelIndex();
			if (farthestCompletedLevelIndex >= 0)
			{
				return database.LevelStubs[this.GetFarthestCompletedLevelIndex()].humanNumber;
			}
			return 0;
		}

		public int GetFarthestUnlockedLevelHumanNumber()
		{
			return this.GetDatabase().LevelStubs[this.GetFarthestUnlockedLevelIndex()].humanNumber;
		}

		public LevelProxy GetFarthestUnlockedLevelProxy()
		{
			LevelProxy farthestCompletedLevelProxy = this.GetFarthestCompletedLevelProxy();
			if (farthestCompletedLevelProxy == LevelProxy.Invalid)
			{
				return new LevelProxy(this.GetDatabase(), new int[1]);
			}
			if (farthestCompletedLevelProxy.NextLevel.IsValid)
			{
				return farthestCompletedLevelProxy.NextLevel;
			}
			return farthestCompletedLevelProxy;
		}

		public MainLevelDatabase GetDatabase()
		{
			return this.levelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		}

		public int GetMaxAvailableLevelHumanNumber()
		{
			int maxAvailableLevel = this.MaxAvailableLevel;
			return this.GetDatabase().DotIndexToNonGateIndex(new LevelProxy(this.GetDatabase(), new int[]
			{
				maxAvailableLevel
			}), null) + 1;
		}

		public int MaxAvailableLevel
		{
			get
			{
				MainLevelDatabase database = this.GetDatabase();
				int val = database.LevelStubs.Count - 1;
				int num = database.WeekOneMaxAvailableHumanLevel;
				if (this.levelAmountProvider != null && this.levelAmountProvider.GetMaxAvailableHumanNumber() > 0)
				{
					num = this.levelAmountProvider.GetMaxAvailableHumanNumber();
				}
				if (database.MaxSupportedHumanLevel > 0 && num > database.MaxSupportedHumanLevel)
				{
					num = database.MaxSupportedHumanLevel;
				}
				int val2 = database.NonGateIndexToDotIndex(database.GetLevel(num - 1), typeof(GateMetaData));
				return Math.Min(val, val2);
			}
		}

		public int GetHardStars(LevelProxy levelProxy)
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			string persistedKey = this.GetPersistedKey(levelProxy);
			if (persistableState.HardStars != null && persistableState.HardStars.ContainsKey(persistedKey))
			{
				return persistableState.HardStars[persistedKey];
			}
			return 0;
		}

		public void SetHardStars(LevelProxy levelProxy, int stars)
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			string persistedKey = this.GetPersistedKey(levelProxy);
			if (persistableState.HardStars == null)
			{
				persistableState.HardStars = new Dictionary<string, int>();
			}
			if (!persistableState.HardStars.ContainsKey(persistedKey) || persistableState.HardStars[persistedKey] < stars)
			{
				persistableState.HardStars[persistedKey] = stars;
			}
		}

		private void RetroactivelyAssignHardStars()
		{
			MainProgressionManager.PersistableState persistableState = this.dataProvider.GetPersistableState();
			MainLevelDatabase database = this.GetDatabase();
			if (persistableState.HardStars == null)
			{
				persistableState.HardStars = new Dictionary<string, int>();
				foreach (KeyValuePair<string, MainProgressionManager.PersistedLevelContextData> keyValuePair in persistableState.Levels)
				{
					int num;
					if (int.TryParse(keyValuePair.Key, out num) && num > 0 && num < database.LevelDifficultyList.Count && database.LevelDifficultyList[num] == LevelDifficulty.Hard)
					{
						persistableState.HardStars[keyValuePair.Key] = keyValuePair.Value.Stars;
					}
				}
			}
		}

		private int progressionGapThreshold = int.MaxValue;

		private readonly GateManager gateManager;

		private readonly LevelReleaseManager levelReleaseManager;

		private readonly MainProgressionManager.IDataProvider dataProvider;

		private readonly LevelDatabaseCollection levelDatabaseCollection;

		private ILevelAmountProvider levelAmountProvider;

		[SettingsProvider("le", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<MainProgressionManager.PersistableState>, IPersistableState
		{
			public PersistableState()
			{
				this.Levels = new Dictionary<string, MainProgressionManager.PersistedLevelContextData>();
				this._farthestCompletedLevelId = -1;
			}

			[JsonSerializable("levels", typeof(MainProgressionManager.PersistedLevelContextData))]
			public Dictionary<string, MainProgressionManager.PersistedLevelContextData> Levels { get; set; }

			[JsonSerializable("hardStars", typeof(int))]
			public Dictionary<string, int> HardStars { get; set; }

			[JsonSerializable("map", null)]
			public int _farthestCompletedLevelId { get; set; }

			public void MergeFromOther(MainProgressionManager.PersistableState other, MainProgressionManager.PersistableState lastCloudState)
			{
				foreach (KeyValuePair<string, MainProgressionManager.PersistedLevelContextData> keyValuePair in other.Levels)
				{
					if (!this.Levels.ContainsKey(keyValuePair.Key))
					{
						this.Levels.Add(keyValuePair.Key, keyValuePair.Value);
					}
					else
					{
						this.Levels[keyValuePair.Key].Points = Mathf.Max(keyValuePair.Value.Points, this.Levels[keyValuePair.Key].Points);
						this.Levels[keyValuePair.Key].Stars = Mathf.Max(keyValuePair.Value.Stars, this.Levels[keyValuePair.Key].Stars);
					}
				}
				if (other.HardStars != null)
				{
					if (this.HardStars == null)
					{
						this.HardStars = new Dictionary<string, int>();
					}
					foreach (KeyValuePair<string, int> keyValuePair2 in other.HardStars)
					{
						string key = keyValuePair2.Key;
						if (!this.HardStars.ContainsKey(key) || this.HardStars[key] < keyValuePair2.Value)
						{
							this.HardStars[key] = keyValuePair2.Value;
						}
					}
				}
				this.FarthestCompletedLevelId = this.CalculateFarthestCompletedLevelId();
			}

			public void EnsureValid()
			{
				MainLevelDatabase database = MainProgressionManager.Instance.GetDatabase();
				LevelProxy nextLevel = database.GetLevel(this.FarthestCompletedLevelId).NextLevel;
				if (!nextLevel.IsValid)
				{
					return;
				}
				if (nextLevel.IsCompleted)
				{
					MainProgressionManager.Instance.dataProvider.SaveUserSettings();
				}
			}

			public int FarthestCompletedLevelId
			{
				get
				{
					return Mathf.Min(this._farthestCompletedLevelId, MainProgressionManager.Instance.MaxAvailableLevel);
				}
				private set
				{
					this._farthestCompletedLevelId = value;
				}
			}

			public void SetFarthestLevelId(int i)
			{
				this.FarthestCompletedLevelId = i;
			}

			public void RefreshFarthestCompletedLevelId()
			{
				this.FarthestCompletedLevelId = this.CalculateFarthestCompletedLevelId();
			}

			private int CalculateFarthestCompletedLevelId()
			{
				MainLevelDatabase database = MainProgressionManager.Instance.GetDatabase();
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < database.NumberOfAvailableLevels; i++)
				{
					LevelProxy level = database.GetLevel(i);
					if (level.IsCompleted)
					{
						num = Mathf.Max(num, i);
						num2 = 0;
					}
					else if (++num2 >= MainProgressionManager.Instance.progressionGapThreshold)
					{
						break;
					}
				}
				return num;
			}
		}

		[SettingsProvider("lePublic", true, new Type[]
		{

		})]
		public class PublicState : IPersistableState<MainProgressionManager.PublicState>, IPersistableState
		{
			[JsonSerializable("at", null)]
			public int LatestUnlockedIndex { get; set; }

			public void MergeFromOther(MainProgressionManager.PublicState other, MainProgressionManager.PublicState lastCloudState)
			{
				this.LatestUnlockedIndex = Mathf.Max(this.LatestUnlockedIndex, other.LatestUnlockedIndex);
			}
		}

		public class PersistedLevelContextData : ILevelAccomplishment
		{
			[JsonSerializable("stars", null)]
			public int Stars { get; set; }

			[JsonSerializable("points", null)]
			public int Points { get; set; }
		}

		public interface IDataProvider
		{
			MainProgressionManager.PersistableState GetPersistableState();

			void HandleDeveloperCheated();

			void SetPublicFarthestLevelId(int farthestLevelId);

			void SaveUserSettings();

			string GetPersistedKeyForLevelProxy(LevelProxy levelProxy);
		}
	}
}
