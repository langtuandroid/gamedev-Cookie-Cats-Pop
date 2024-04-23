using System;
using System.Collections.Generic;
using ConfigSchema;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	public class HotStreakMetaData : FeatureMetaData
	{
		public HotStreakMetaData()
		{
			this.Tiers = new List<HotStreakTier>();
		}

		[Required]
		[Description("Availabel Hot Streak tiers")]
		[JsonSerializable("Tiers", typeof(HotStreakTier))]
		public List<HotStreakTier> Tiers { get; set; }
	}
}
