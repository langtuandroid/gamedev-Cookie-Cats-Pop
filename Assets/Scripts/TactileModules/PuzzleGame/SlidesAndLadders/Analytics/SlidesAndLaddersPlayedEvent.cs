using System;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Analytics
{
	[TactileAnalytics.EventAttribute("snlPlayed", true)]
	public class SlidesAndLaddersPlayedEvent : BasicEvent
	{
		public SlidesAndLaddersPlayedEvent(int levelNumber, bool ladderUsed, bool slideUsed, int movedUp, int movedDown, int chestRank, string rewardType, int rewardAmount)
		{
			this.LevelNumber = levelNumber;
			this.LadderUsed = ladderUsed;
			this.SlideUsed = slideUsed;
			this.MovedUp = movedUp;
			this.MovedDown = movedDown;
			this.ChestRank = chestRank;
			this.RewardAmount = rewardAmount;
			if (!string.IsNullOrEmpty(rewardType))
			{
				this.RewardType = rewardType;
			}
		}

		private TactileAnalytics.RequiredParam<int> LevelNumber { get; set; }

		private TactileAnalytics.RequiredParam<bool> LadderUsed { get; set; }

		private TactileAnalytics.RequiredParam<bool> SlideUsed { get; set; }

		private TactileAnalytics.RequiredParam<int> MovedUp { get; set; }

		private TactileAnalytics.RequiredParam<int> MovedDown { get; set; }

		private TactileAnalytics.RequiredParam<int> ChestRank { get; set; }

		private TactileAnalytics.RequiredParam<int> RewardAmount { get; set; }

		private TactileAnalytics.OptionalParam<string> RewardType { get; set; }
	}
}
