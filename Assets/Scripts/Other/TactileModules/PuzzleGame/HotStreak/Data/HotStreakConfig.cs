using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.HotStreak.Data
{
	[ConfigProvider("HotStreakConfig")]
	public class HotStreakConfig
	{
		[Required]
		[Description("Kill switch")]
		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[Required]
		[Description("The unlocked human number level requirement to play Hot Streak from the feature scheduler")]
		[JsonSerializable("LevelRequiredGlobal", null)]
		public int LevelRequiredGlobal { get; set; }

		[Required]
		[Description("The unlocked human number level requirement to play Hot Streak locally")]
		[JsonSerializable("LevelRequiredLocal", null)]
		public int LevelRequiredLocal { get; set; }

		[Required]
		[Description("Duration used in the context on local Hot Streak ONLY. Duration for normal hot streaks is defined in the scheduler.")]
		[JsonSerializable("MaxPlayTimeDuration", null)]
		public int MaxPlayTimeDurationLocal { get; set; }

		[Required]
		[Description("Availabel Hot Streak tiers")]
		[JsonSerializable("Tiers", typeof(HotStreakTier))]
		public List<HotStreakTier> Tiers { get; set; }

		[Required]
		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
