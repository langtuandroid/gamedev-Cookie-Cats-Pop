using System;

namespace Tactile.GardenGame.Story.Rewards
{
	public interface IStoryRewardsFactory
	{
		StoryRewardFlow CreateStoryRewardFlow();

		IStoryRewards CreateStoryRewards();
	}
}
