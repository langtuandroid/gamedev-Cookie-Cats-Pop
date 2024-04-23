using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("AchievementsConfig")]
public class AchievementsConfig
{
	[JsonSerializable("LevelRequiredForNotification", null)]
	public int LevelRequiredForNotification { get; set; }

	[JsonSerializable("Rewards", typeof(AchievementReward))]
	public List<AchievementReward> Rewards { get; set; }
}
