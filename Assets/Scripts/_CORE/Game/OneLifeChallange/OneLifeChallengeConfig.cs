using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

[ObsoleteJsonName(new string[]
{
	"Period",
	"RewardPackage"
})]
[ConfigProvider("OneLifeChallengeConfig")]
public class OneLifeChallengeConfig
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
