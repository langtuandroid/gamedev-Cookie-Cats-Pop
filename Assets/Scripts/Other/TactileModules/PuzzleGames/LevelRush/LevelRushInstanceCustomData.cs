using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushInstanceCustomData : FeatureInstanceCustomData
	{
		public LevelRushInstanceCustomData()
		{
			this.StartLevel = -1;
			this.Progress = -1;
			this.LatestRewardClaimed = -1;
		}

		[JsonSerializable("sl", null)]
		public int StartLevel { get; set; }

		[JsonSerializable("rl", null)]
		public int Progress { get; set; }

		[JsonSerializable("lrc", null)]
		public int LatestRewardClaimed { get; set; }
	}
}
