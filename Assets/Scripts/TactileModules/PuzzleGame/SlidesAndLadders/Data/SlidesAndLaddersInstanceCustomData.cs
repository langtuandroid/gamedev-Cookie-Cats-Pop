using System;
using System.Collections.Generic;
using TactileModules.MapFeature;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	public class SlidesAndLaddersInstanceCustomData : MapFeatureInstanceCustomData
	{
		public SlidesAndLaddersInstanceCustomData()
		{
			this.Reset();
		}

		[JsonSerializable("state", null)]
		public ResultState State { get; set; }

		[JsonSerializable("progress", null)]
		public int FarthestUnlockedLevelIndex { get; set; }

		[JsonSerializable("current", null)]
		public int CurrentLevelIndex { get; set; }

		[JsonSerializable("c", typeof(int))]
		public List<int> RewardsClaimed { get; set; }

		[JsonSerializable("nws", null)]
		public int NumberOfCurrentWheelSpins { get; set; }

		[JsonSerializable("csw", null)]
		public bool CanSpinWheel { get; set; }

		[JsonSerializable("hst", null)]
		public bool HasShownTutorial { get; set; }

		[JsonSerializable("adr", typeof(ItemAmount))]
		public List<ItemAmount> AddedChestRewards { get; set; }

		[JsonSerializable("cf", null)]
		public bool CompletedFeature { get; set; }

		[JsonSerializable("sc", null)]
		public int SpinCount { get; set; }

		public void Reset()
		{
			this.FarthestUnlockedLevelIndex = 0;
			this.CurrentLevelIndex = 0;
			this.NumberOfCurrentWheelSpins = 1;
			this.SpinCount = 0;
			this.CanSpinWheel = false;
			this.HasShownTutorial = false;
			this.RewardsClaimed = new List<int>();
			this.AddedChestRewards = new List<ItemAmount>();
			this.CompletedFeature = false;
			this.State = ResultState.None;
		}

		public static void TakeState<FeatureState>(ref FeatureState toMerge, FeatureState state) where FeatureState : SlidesAndLaddersInstanceCustomData
		{
			toMerge.FarthestUnlockedLevelIndex = state.FarthestUnlockedLevelIndex;
			toMerge.CurrentLevelIndex = state.CurrentLevelIndex;
			toMerge.State = state.State;
			toMerge.RewardsClaimed = state.RewardsClaimed;
			toMerge.NumberOfCurrentWheelSpins = state.NumberOfCurrentWheelSpins;
			toMerge.CanSpinWheel = state.CanSpinWheel;
			toMerge.HasShownTutorial = state.HasShownTutorial;
			toMerge.AddedChestRewards = state.AddedChestRewards;
		}
	}
}
