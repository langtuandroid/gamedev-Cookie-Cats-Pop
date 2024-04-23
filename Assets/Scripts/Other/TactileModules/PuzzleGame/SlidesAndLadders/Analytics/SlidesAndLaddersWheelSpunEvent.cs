using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Analytics
{
	[TactileAnalytics.EventAttribute("snlWheelSpun", true)]
	public class SlidesAndLaddersWheelSpunEvent : BasicEvent
	{
		public SlidesAndLaddersWheelSpunEvent(int levelNumber, string wheelResult, int movedUp, bool chestReached, int chestRank, int spinCount, bool ladderReached, bool slideReached, int finalChestCoinsAmount, string finalChestReward)
		{
			this.LevelNumber = levelNumber;
			this.WheelResult = wheelResult;
			this.MovedUp = movedUp;
			this.ChestReached = chestReached;
			this.ChestRank = chestRank;
			this.SpinCount = spinCount;
			this.LadderReached = ladderReached;
			this.SlideReached = slideReached;
			this.FinalChestCoinsAmount = finalChestCoinsAmount;
			if (!string.IsNullOrEmpty(finalChestReward))
			{
				this.FinalChestReward = finalChestReward;
			}
		}

		private TactileAnalytics.RequiredParam<int> LevelNumber { get; set; }

		private TactileAnalytics.RequiredParam<string> WheelResult { get; set; }

		private TactileAnalytics.RequiredParam<int> MovedUp { get; set; }

		private TactileAnalytics.RequiredParam<bool> ChestReached { get; set; }

		private TactileAnalytics.RequiredParam<int> ChestRank { get; set; }

		private TactileAnalytics.RequiredParam<int> SpinCount { get; set; }

		private TactileAnalytics.RequiredParam<bool> LadderReached { get; set; }

		private TactileAnalytics.RequiredParam<bool> SlideReached { get; set; }

		private TactileAnalytics.RequiredParam<int> FinalChestCoinsAmount { get; set; }

		private TactileAnalytics.OptionalParam<string> FinalChestReward { get; set; }
	}
}
