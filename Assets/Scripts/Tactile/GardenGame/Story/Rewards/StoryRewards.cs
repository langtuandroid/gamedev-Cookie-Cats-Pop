using System;
using System.Collections.Generic;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.UserSettings;

namespace Tactile.GardenGame.Story.Rewards
{
	public class StoryRewards : IStoryRewards
	{
		public StoryRewards(IConfigGetter<StoryConfig> config, IUserSettingsGetter<StoryManager.PersistableState> state, IStoryManager storyManager)
		{
			this.config = config;
			this.state = state;
			this.storyManager = storyManager;
		}

		public List<StoryConfig.Reward> GetClaimableRewards()
		{
			List<StoryConfig.Reward> list = new List<StoryConfig.Reward>();
			List<StoryConfig.Reward> list2 = this.config.Get().StoryRewards.FindAll((StoryConfig.Reward r) => r.Chapter == this.state.Get().CurrentChapter);
			float num = (float)this.state.Get().CurrentChapter + this.storyManager.GetChapterProgressionNormalized();
			float num2 = (float)this.state.Get().CurrentChapter + this.storyManager.GetPreviousChapterProgressionNormalized();
			foreach (StoryConfig.Reward reward in list2)
			{
				float num3 = (float)this.state.Get().CurrentChapter + reward.NormalizedProgression;
				if (num3 > num2 && num >= num3 && num3 > this.state.Get().LastClaimedReward)
				{
					list.Add(reward);
				}
			}
			return list;
		}

		public List<StoryConfig.Reward> GetVisibleChapterRewards()
		{
			List<StoryConfig.Reward> list = new List<StoryConfig.Reward>();
			List<StoryConfig.Reward> list2 = new List<StoryConfig.Reward>();
			if (this.config.Get().StoryRewards != null && this.config.Get().StoryRewards.Count > 0)
			{
				list2 = this.config.Get().StoryRewards.FindAll((StoryConfig.Reward r) => r.Chapter == this.state.Get().CurrentChapter);
			}
			float num = (!string.IsNullOrEmpty(this.state.Get().LastCompletedTask)) ? this.storyManager.GetPreviousChapterProgressionNormalized() : this.storyManager.GetChapterProgressionNormalized();
			float num2 = (float)this.state.Get().CurrentChapter + num;
			foreach (StoryConfig.Reward reward in list2)
			{
				float num3 = (float)this.state.Get().CurrentChapter + reward.NormalizedProgression;
				if (num3 > num2)
				{
					list.Add(reward);
				}
			}
			return list;
		}

		public void SaveLastClaimedReward()
		{
			this.state.Get().SetLastClaimedReward((float)this.state.Get().CurrentChapter + this.storyManager.GetChapterProgressionNormalized());
		}

		private readonly IConfigGetter<StoryConfig> config;

		private readonly IUserSettingsGetter<StoryManager.PersistableState> state;

		private readonly IStoryManager storyManager;
	}
}
