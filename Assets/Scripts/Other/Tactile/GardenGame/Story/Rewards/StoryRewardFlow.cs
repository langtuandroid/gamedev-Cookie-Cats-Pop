using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.GameCore.Rewards;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Story.Rewards
{
	public class StoryRewardFlow : IFlow, IFiberRunnable
	{
		public StoryRewardFlow(IStoryRewards storyRewards, IRewardsFactory rewardsFactory, UserSettingsManager userSettingsManager)
		{
			this.storyRewards = storyRewards;
			this.rewardsFactory = rewardsFactory;
			this.userSettingsManager = userSettingsManager;
		}

		public IEnumerator Run()
		{
			List<StoryConfig.Reward> rewards = this.storyRewards.GetClaimableRewards();
			if (rewards.Count > 0)
			{
				List<ItemAmount> rewardItems = new List<ItemAmount>();
				foreach (StoryConfig.Reward reward in rewards)
				{
					rewardItems.AddRange(reward.Items);
				}
				this.storyRewards.SaveLastClaimedReward();
				IGiveAndAnimateRewards giveAndAnimateRewards = this.rewardsFactory.CreateGiveAndAnimateRewards();
				yield return giveAndAnimateRewards.Gifts(rewardItems, rewards[0].RewardType, "storyRewardGifts");
				this.userSettingsManager.SaveLocalSettings();
			}
			yield break;
		}

		public void OnExit()
		{
		}

		private readonly IStoryRewards storyRewards;

		private readonly IRewardsFactory rewardsFactory;

		private readonly UserSettingsManager userSettingsManager;
	}
}
