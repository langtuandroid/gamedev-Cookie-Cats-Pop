using System;

public static class AchievementsConfigExtensions
{
	public static AchievementReward GetReward(this AchievementsConfig config, string id)
	{
		foreach (AchievementReward achievementReward in config.Rewards)
		{
			if (achievementReward.ID == id)
			{
				return achievementReward;
			}
		}
		return null;
	}
}
