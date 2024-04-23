using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public class TreasureHuntInstanceCustomData : FeatureInstanceCustomData
	{
		public TreasureHuntInstanceCustomData()
		{
			this.farthestCompletedLevel = -1;
			this.rewardClaimed = false;
		}

		[JsonSerializable("progress", null)]
		public int farthestCompletedLevel { get; set; }

		[JsonSerializable("c", null)]
		public bool rewardClaimed { get; set; }
	}
}
