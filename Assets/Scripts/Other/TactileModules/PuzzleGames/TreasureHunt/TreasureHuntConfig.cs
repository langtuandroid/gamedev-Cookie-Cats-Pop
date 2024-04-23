using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	[ObsoleteJsonName(new string[]
	{
		"TressureHuntPeriod",
		"StartDate",
		"EventDuration",
		"JoinWindowDuration",
		"MaxPlayTimeDuration",
		"RewardPackage"
	})]
	[ConfigProvider("TreasureHuntConfig")]
	public class TreasureHuntConfig
	{
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[JsonSerializable("Enabled", null)]
		public bool FeatureEnabled { get; set; }

		[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
		[JsonSerializable("Rewards", typeof(ItemAmount))]
		public List<ItemAmount> Rewards { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
