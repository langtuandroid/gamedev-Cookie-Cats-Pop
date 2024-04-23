using System;
using System.Collections.Generic;

namespace Tactile.GardenGame.Story.Rewards
{
	public interface IStoryRewards
	{
		List<StoryConfig.Reward> GetClaimableRewards();

		List<StoryConfig.Reward> GetVisibleChapterRewards();

		void SaveLastClaimedReward();
	}
}
