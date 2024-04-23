using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelRush
{
	[ConfigProvider("LevelRushConfig")]
	[ObsoleteJsonName(new string[]
	{
		"StartDate",
		"JoinWindowDuration",
		"EventDuration"
	})]
	public class LevelRushConfig
	{
		[Required]
		[Description("Kill switch")]
		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[Description("The unlocked human number level requirement to play Level Rushes from the feature scheduler. Local level rushes does not consider this value.")]
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[Description("Duration used in the context on local Level Rush ONLY. Duration for normal level rushes is defined in the scheduler.")]
		[JsonSerializable("MaxPlayTimeDuration", null)]
		public int MaxPlayTimeDurationLocal { get; set; }

		[Description("Use any trigger levels? Trigger levels are used for starting local Level Rushes")]
		[JsonSerializable("UseTriggerLevels", null)]
		public bool UseTriggerLevels { get; set; }

		[ArrayFormat(ArrayFormatAttribute.ArrayFormat.table)]
		[Description("Array of trigger levels.")]
		[JsonSerializable("TriggerLevels", typeof(int))]
		public List<int> TriggerLevels { get; set; }

		[Required]
		[Description("The rewards for completing certain levels in level rush. Rewards are global accross all Level Rushes, both local and not.")]
		[JsonSerializable("Rewards", typeof(LevelRushConfig.Reward))]
		public List<LevelRushConfig.Reward> LevelRushRewards { get; set; }

		[Required]
		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }

		[RequireAll]
		[ObsoleteJsonName("Package")]
		public class Reward
		{
			[Description("The level index of this reward relative to the start level of the Level Rush.")]
			[JsonSerializable("LevelIndex", null)]
			public int LevelIndex { get; set; }

			[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
			[JsonSerializable("Items", typeof(ItemAmount))]
			public List<ItemAmount> Items { get; set; }
		}
	}
}
