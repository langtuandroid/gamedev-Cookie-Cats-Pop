using System;
using TactileModules.GameCore.Rewards;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.UserSettings;

namespace Tactile.GardenGame.Story.Rewards
{
	public class StoryRewardsFactory : IStoryRewardsFactory
	{
		public StoryRewardsFactory(IConfigGetter<StoryConfig> config, IUserSettingsGetter<StoryManager.PersistableState> state, IStoryManager storyManager, IRewardsFactory rewardsFactory, UserSettingsManager userSettingsManager)
		{
			this.userSettingsManager = userSettingsManager;
			this.rewardsFactory = rewardsFactory;
			this.storyRewards = new StoryRewards(config, state, storyManager);
		}

		public StoryRewardFlow CreateStoryRewardFlow()
		{
			return new StoryRewardFlow(this.storyRewards, this.rewardsFactory, this.userSettingsManager);
		}

		public IStoryRewards CreateStoryRewards()
		{
			return this.storyRewards;
		}

		private readonly IStoryRewards storyRewards;

		private readonly IRewardsFactory rewardsFactory;

		private readonly UserSettingsManager userSettingsManager;
	}
}
