using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	public class HotStreakInstanceCustomData : FeatureInstanceCustomData
	{
		public HotStreakInstanceCustomData()
		{
			this.Progress = 0;
		}

		[JsonSerializable("pr", null)]
		public int Progress { get; set; }
	}
}
