using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Data
{
	[ConfigProvider("SlidesAndLaddersConfig")]
	public class SlidesAndLaddersConfig
	{
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }

		[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
		[JsonSerializable("LevelRewards", typeof(ItemAmount))]
		public List<ItemAmount> LevelRewards { get; set; }

		[HeaderTemplate("{{ self.Type }} x {{ self.Amount }}")]
		[JsonSerializable("SlidesRewards", typeof(ItemAmount))]
		public List<ItemAmount> SlidesRewards { get; set; }
	}
}
