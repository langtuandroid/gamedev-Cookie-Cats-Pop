using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public class SlidesAndLaddersFeatureProgression : ISlidesAndLaddersFeatureProgression
	{
		public SlidesAndLaddersFeatureProgression(IDataProvider<SlidesAndLaddersInstanceCustomData> customData, ISlidesAndLaddersLevelDatabase levelDatabase)
		{
			this.customData = customData;
			this.levelDatabase = levelDatabase;
		}

		public ResultState ResultState
		{
			get
			{
				return this.customData.Get().State;
			}
			private set
			{
				this.customData.Get().State = value;
			}
		}

		public int CurrentLevelIndex
		{
			get
			{
				return this.customData.Get().CurrentLevelIndex;
			}
			set
			{
				this.customData.Get().CurrentLevelIndex = value;
			}
		}

		public int FarthestUnlockedLevelIndex
		{
			get
			{
				return this.customData.Get().FarthestUnlockedLevelIndex;
			}
			set
			{
				this.customData.Get().FarthestUnlockedLevelIndex = value;
			}
		}

		public bool HasShownTutorial
		{
			get
			{
				return this.customData.Get().HasShownTutorial;
			}
			set
			{
				this.customData.Get().HasShownTutorial = value;
			}
		}

		public bool CompletedFeature
		{
			get
			{
				return this.customData.Get().CompletedFeature;
			}
			set
			{
				this.customData.Get().CompletedFeature = value;
			}
		}

		public void SetResultState(ResultState state)
		{
			this.ResultState = state;
		}

		public bool IsReadyToPlayLevel(ILevelProxy level)
		{
			return !this.customData.Get().CanSpinWheel && level.LevelAsset is LevelAsset;
		}

		public bool IsLevelIndexEndChest(int levelIndex)
		{
			return this.levelDatabase.GetLevelProxy(levelIndex).LevelAsset is RewardLevelAsset;
		}

		public int FeatureSpinCount()
		{
			return this.customData.Get().SpinCount;
		}

		private readonly IDataProvider<SlidesAndLaddersInstanceCustomData> customData;

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;
	}
}
