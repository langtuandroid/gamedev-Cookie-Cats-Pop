using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	[ConfigProvider("StoryMapEventConfig")]
	public class StoryMapEventConfig
	{
		[Required]
		[Description("Kill switch")]
		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[Description("The human level required to unlock the feature.")]
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[Description("Seconds to pass between reminders.")]
		[JsonSerializable("ReminderCooldown", null)]
		public int ReminderCooldown { get; set; }

		[Required]
		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }

		[Required]
		[Description("Chapter rewards")]
		[JsonSerializable("Rewards", typeof(StoryMapEventConfig.Reward))]
		public List<StoryMapEventConfig.Reward> ChapterRewards { get; set; }

		[RequireAll]
		public class Reward
		{
			[JsonSerializable("Chapter", null)]
			public int Chapter { get; set; }

			[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
			[JsonSerializable("Items", typeof(ItemAmount))]
			public List<ItemAmount> Items { get; set; }
		}
	}
}
