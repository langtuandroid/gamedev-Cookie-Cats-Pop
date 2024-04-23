using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using TactileModules.PuzzleGame.ThemeHunt.Popup;
using UnityEngine;

namespace TactileModules.PuzzleGame.ThemeHunt
{
	public abstract class ThemeHuntManagerBase
	{
		protected ThemeHuntManagerBase()
		{
			this.RegisterPopups();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnItemSpawn;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnHuntEnded;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnCollectThemeItem;

		protected abstract int GetFarthestUnlockedLevelHumanNumber();

		protected abstract ThemeHuntConfig GetThemeHuntConfig();

		protected abstract void RewardsClaimed(List<ThemeHuntRewardItem> rewards);

		protected abstract void LogThemeHuntItemCollected(string themeId, int collectedItems, int itemId);

		protected abstract MapViewSetup GetMapViewSetup();

		protected abstract void SaveLocalPrivateUserSettings();

		protected abstract void RegisterUserSettingsSync(Action handler);

		protected abstract ThemeHuntManagerBase.PersistableState State { get; }

		public static T CreateInstance<T>() where T : ThemeHuntManagerBase, new()
		{
			ThemeHuntManagerBase.Instance = Activator.CreateInstance<T>();
			ThemeHuntManagerBase.Instance.Inititalize();
			return (T)((object)ThemeHuntManagerBase.Instance);
		}

		protected virtual void RegisterPopups()
		{
			MapPopupManager.Instance.RegisterPopupObject(new ThemeHuntEndPopup(this));
			MapPopupManager.Instance.RegisterPopupObject(new ThemeHuntStartPopup(this));
		}

		protected virtual void Inititalize()
		{
			this.activeThemeItems = ThemeHuntManagerBase.PersistedActiveThemeItems;
			TimeStampManager.Instance.TimeDone += this.HandleTimeDone;
			this.RefreshActiveThemeItemsList();
			this.RegisterUserSettingsSync(new Action(this.HandleUserSettingsSynced));
		}

		public static ThemeHuntManagerBase Instance { get; private set; }

		public bool IsHuntConfigured(string id)
		{
			return this.GetThemeHuntConfig().GetHunt(id) != null;
		}

		public bool IsHuntActiveOnClient()
		{
			return this.State.ID == this.GetClientVersionId() && this.State.HuntIsActive;
		}

		private int TotalNumberOfSpawners
		{
			get
			{
				return this.spawnIds.Count;
			}
		}

		public bool ShouldEndHunt()
		{
			return this.IsANewVersion() && this.State.HuntIsActive;
		}

		public bool ShouldStartHunt()
		{
			return this.IsHuntConfigured(this.GetClientVersionId()) && this.IsANewVersion() && !this.State.HuntIsActive && this.GetFarthestUnlockedLevelHumanNumber() >= this.GetThemeHuntConfig().LevelRequired;
		}

		protected void CleanUpHunt()
		{
			this.activeThemeItems.Clear();
			TimeStampManager.Instance.RemoveTimeStampIfItExist("ThemeHuntTimeStamp");
			if (this.OnHuntEnded != null)
			{
				this.OnHuntEnded();
			}
		}

		public void EndHunt()
		{
			this.CleanUpHunt();
			this.State.HuntIsActive = false;
			this.Save();
		}

		public void StartHunt()
		{
			this.ResetThemeHuntState();
			this.State.ID = this.GetClientVersionId();
			this.State.HuntIsActive = true;
			this.SpawnThemeItems(this.MaxSpawnItems);
			this.Save();
		}

		protected void HandleUserSettingsSynced()
		{
			if (!this.IsHuntActiveOnClient())
			{
				this.CleanUpHunt();
			}
			if (this.IsHuntActiveOnClient() && this.activeThemeItems.Count != this.MaxSpawnItems && !TimeStampManager.Instance.TimeStampExist("ThemeHuntTimeStamp"))
			{
				this.activeThemeItems.Clear();
				this.SpawnThemeItems(this.MaxSpawnItems);
			}
		}

		public static int GetSubVersionNumber(string version)
		{
			int result = 0;
			int.TryParse(version.Split(new char[]
			{
				'.'
			})[1], out result);
			return result;
		}

		private bool IsANewVersion()
		{
			return ThemeHuntManagerBase.GetSubVersionNumber(this.GetClientVersionId()) > ThemeHuntManagerBase.GetSubVersionNumber(this.State.ID);
		}

		private string GetClientVersionId()
		{
			return SystemInfoHelper.MajorAndMinorVersion + ".x";
		}

		public ThemeHuntRewardItem GetNextThemeHuntReward()
		{
			List<ThemeHuntRewardItem> rewards = this.Rewards;
			if (rewards != null)
			{
				return rewards[Math.Min(this.State.ClaimedRewards, rewards.Count - 1)];
			}
			return null;
		}

		public float HuntProgress
		{
			get
			{
				int totalItemsRequiredForHunt = this.TotalItemsRequiredForHunt;
				if (totalItemsRequiredForHunt == 0)
				{
					return 0f;
				}
				return (float)this.State.CollectedItems / (float)totalItemsRequiredForHunt;
			}
		}

		public void RefreshActiveThemeItemsList()
		{
			List<MapElementData> list = this.GetMapViewSetup().mapElementData.FindAll((MapElementData m) => m.prefabName == "MapThemeHuntItemSpawner");
			this.spawnIds.Clear();
			foreach (MapElementData mapElementData in list)
			{
				Hashtable hashtable = mapElementData.instantiatorDataAsJSON.hashtableFromJson();
				this.spawnIds.Add(int.Parse(hashtable["Id"].ToString()));
			}
			this.activeThemeItems.RemoveAll((int t) => !this.spawnIds.Contains(t));
			this.HandleUserSettingsSynced();
			ThemeHuntManagerBase.PersistedActiveThemeItems = this.activeThemeItems;
		}

		private void HandleTimeDone(string timer)
		{
			if (timer == "ThemeHuntTimeStamp")
			{
				this.SpawnThemeItems(this.MaxSpawnItems - this.activeThemeItems.Count);
			}
		}

		private void SpawnThemeItems(int n)
		{
			List<int> list = new List<int>();
			foreach (int item in this.spawnIds)
			{
				if (!this.activeThemeItems.Contains(item))
				{
					list.Add(item);
				}
			}
			list.Shuffle<int>();
			for (int i = 0; i < n; i++)
			{
				if (i >= list.Count)
				{
					break;
				}
				this.activeThemeItems.Add(list[i]);
			}
			if (this.OnItemSpawn != null)
			{
				this.OnItemSpawn();
			}
			this.Save();
		}

		public bool TryCollectThemeItem(int itemId)
		{
			if (!this.activeThemeItems.Contains(itemId))
			{
				return false;
			}
			this.State.CollectedItems++;
			this.activeThemeItems.Remove(itemId);
			if (!TimeStampManager.Instance.TimeStampExist("ThemeHuntTimeStamp"))
			{
				TimeStampManager.Instance.CreateTimeStamp("ThemeHuntTimeStamp", this.SpawnFrequency);
			}
			this.CheckForRewards();
			if (this.OnCollectThemeItem != null)
			{
				this.OnCollectThemeItem();
			}
			this.Save();
			this.LogThemeHuntItemCollected(this.State.ID, this.State.CollectedItems, itemId);
			return true;
		}

		public void CheckForRewards()
		{
			List<ThemeHuntRewardItem> rewards;
			if (this.TryClaimRewards(out rewards))
			{
				this.RewardsClaimed(rewards);
			}
		}

		private bool TryClaimRewards(out List<ThemeHuntRewardItem> outputRewards)
		{
			outputRewards = new List<ThemeHuntRewardItem>();
			List<ThemeHuntRewardItem> rewards = this.Rewards;
			if (rewards == null)
			{
				return false;
			}
			for (int i = 0; i < rewards.Count; i++)
			{
				ThemeHuntRewardItem themeHuntRewardItem = rewards[i];
				if (themeHuntRewardItem.ThemeItemsRequired <= this.State.CollectedItems && this.State.ClaimedRewards <= i)
				{
					this.State.ClaimedRewards++;
					outputRewards.Add(themeHuntRewardItem);
				}
			}
			if (outputRewards.Count > 0 && this.State.ClaimedRewards == this.Rewards.Count)
			{
				this.EndHunt();
			}
			return outputRewards.Count > 0;
		}

		public int TotalItemsRequiredForHunt
		{
			get
			{
				List<ThemeHuntRewardItem> rewards = this.Rewards;
				if (rewards != null && rewards.Count > 0)
				{
					return rewards[rewards.Count - 1].ThemeItemsRequired;
				}
				return 0;
			}
		}

		public List<ThemeHuntRewardItem> Rewards
		{
			get
			{
				ThemeHuntEntryConfig hunt = this.GetThemeHuntConfig().GetHunt(this.State.ID);
				if (hunt != null)
				{
					List<ThemeHuntRewardItem> rewards = hunt.Rewards;
					rewards.Sort((ThemeHuntRewardItem a, ThemeHuntRewardItem b) => a.ThemeItemsRequired.CompareTo(b.ThemeItemsRequired));
					return rewards;
				}
				return null;
			}
		}

		public int GetClaimedRewards
		{
			get
			{
				return this.State.ClaimedRewards;
			}
		}

		public int GetCollectedItems
		{
			get
			{
				return this.State.CollectedItems;
			}
		}

		public List<int> GetActiveThemeItems
		{
			get
			{
				return this.activeThemeItems;
			}
		}

		private int SpawnFrequency
		{
			get
			{
				ThemeHuntEntryConfig hunt = this.GetThemeHuntConfig().GetHunt(this.State.ID);
				if (hunt != null)
				{
					return hunt.SpawnFrequency;
				}
				return 1;
			}
		}

		private int MaxSpawnItems
		{
			get
			{
				ThemeHuntEntryConfig hunt = this.GetThemeHuntConfig().GetHunt(this.State.ID);
				if (hunt != null)
				{
					return Mathf.Clamp(hunt.MaxSpawnItems, 0, this.TotalNumberOfSpawners);
				}
				return 0;
			}
		}

		private void ResetThemeHuntState()
		{
			this.activeThemeItems = new List<int>();
			this.State.Reset();
			this.Save();
		}

		private void Save()
		{
			ThemeHuntManagerBase.PersistedActiveThemeItems = this.activeThemeItems;
			this.SaveLocalPrivateUserSettings();
		}

		private static List<int> PersistedActiveThemeItems
		{
			get
			{
				string securedString = TactilePlayerPrefs.GetSecuredString("ThemeHuntActiveThemeHuntItems", string.Empty);
				if (securedString.Length > 0)
				{
					return JsonSerializer.ArrayListToGenericList<int>(securedString.arrayListFromJson());
				}
				return new List<int>();
			}
			set
			{
				if (value != null)
				{
					TactilePlayerPrefs.SetSecuredString("ThemeHuntActiveThemeHuntItems", JsonSerializer.GenericListToArrayList<int>(value).toJson());
				}
				else
				{
					TactilePlayerPrefs.SetSecuredString("ThemeHuntActiveThemeHuntItems", string.Empty);
				}
			}
		}

		private const string THEME_HUNT_TIME_STAMP = "ThemeHuntTimeStamp";

		public const string HUNT_INIT_ID = "0.0.0";

		private const string MAP_SPAWNER = "MapThemeHuntItemSpawner";

		private const string PREFS_ACTIVE_THEME_ITEMS = "ThemeHuntActiveThemeHuntItems";

		private List<int> spawnIds = new List<int>();

		protected List<int> activeThemeItems;

		[SettingsProvider("th", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<ThemeHuntManagerBase.PersistableState>, IPersistableState
		{
			public PersistableState()
			{
				this.Reset();
			}

			[JsonSerializable("tid", null)]
			public string ID { get; set; }

			[JsonSerializable("tct", null)]
			public int CollectedItems { get; set; }

			[JsonSerializable("clr", null)]
			public int ClaimedRewards { get; set; }

			[JsonSerializable("hch", null)]
			public bool HuntIsActive { get; set; }

			public void Reset()
			{
				this.ID = "0.0.0";
				this.CollectedItems = 0;
				this.ClaimedRewards = 0;
				this.HuntIsActive = false;
			}

			public void MergeFromOther(ThemeHuntManagerBase.PersistableState newest, ThemeHuntManagerBase.PersistableState last)
			{
				this.MergeOtherIntoThis(newest);
			}

			public void MergeOtherIntoThis(ThemeHuntManagerBase.PersistableState other)
			{
				if (ThemeHuntManagerBase.GetSubVersionNumber(other.ID) > ThemeHuntManagerBase.GetSubVersionNumber(this.ID))
				{
					this.ID = other.ID;
					this.CollectedItems = other.CollectedItems;
					this.ClaimedRewards = other.ClaimedRewards;
					this.HuntIsActive = other.HuntIsActive;
				}
				if (ThemeHuntManagerBase.GetSubVersionNumber(other.ID) == ThemeHuntManagerBase.GetSubVersionNumber(this.ID))
				{
					this.CollectedItems = Math.Max(this.CollectedItems, other.CollectedItems);
					this.ClaimedRewards = Math.Max(this.ClaimedRewards, other.ClaimedRewards);
					this.HuntIsActive = (this.HuntIsActive && other.HuntIsActive);
				}
			}

			public bool DataEqual(ThemeHuntManagerBase.PersistableState other)
			{
				return !(this.ID != other.ID) && this.CollectedItems == other.CollectedItems && this.ClaimedRewards == other.ClaimedRewards && this.HuntIsActive == other.HuntIsActive;
			}
		}
	}
}
