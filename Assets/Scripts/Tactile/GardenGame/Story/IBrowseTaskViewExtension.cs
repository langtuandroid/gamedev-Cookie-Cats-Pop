using System;
using System.Collections;
using Tactile.GardenGame.Story.Rewards;
using TactileModules.GameCore.Inventory;

namespace Tactile.GardenGame.Story
{
	public interface IBrowseTaskViewExtension
	{
		IEnumerator AnimateTaskCompleted(IStoryManager storyManager);

		void AddRewardIndicator(StoryRewardIndicator rewardItem, float progression);

		IFlyingItemsAnimator TaskStarAnimator { get; }
	}
}
