using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	public class HotStreakTypeCustomData : FeatureTypeCustomData
	{
		public HotStreakTypeCustomData()
		{
			this.HasEverStartedLocalFeature = false;
		}

		[JsonSerializable("clf", null)]
		public bool HasEverStartedLocalFeature { get; set; }
	}
}
