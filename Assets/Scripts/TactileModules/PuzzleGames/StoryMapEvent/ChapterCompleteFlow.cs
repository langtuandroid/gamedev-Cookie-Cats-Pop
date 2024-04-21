using System;
using System.Collections;
using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class ChapterCompleteFlow
	{
		public ChapterCompleteFlow(IStoryManager storyManager, ConfigGetter<StoryMapEventConfig> configGetter, ViewFactory viewFactory, UIViewManager uiViewManager, InventoryManager inventoryManager)
		{
			this.storyManager = storyManager;
			this.configGetter = configGetter;
			this.viewFactory = viewFactory;
			this.uiViewManager = uiViewManager;
			this.inventoryManager = inventoryManager;
			storyManager.LastTaskInChapterComplete.Register(new Func<int, bool, IEnumerator>(this.HandleLastTaskInChapterComplete));
		}

		private IEnumerator HandleLastTaskInChapterComplete(int chapter, bool isEndOfStoryContent)
		{
			yield return this.GiveRewards(chapter);
			if (isEndOfStoryContent)
			{
				yield return this.ShowEndOfContent();
			}
			yield break;
		}

		private IEnumerator GiveRewards(int chapter)
		{
			StoryManager.PersistableState storyState = (this.storyManager as StoryManager).State;
			StoryMapEventConfig.Reward reward = this.configGetter.Get().ChapterRewards.Find((StoryMapEventConfig.Reward x) => x.Chapter == chapter);
			if (reward != null && storyState.LastClaimedReward < (float)chapter)
			{
				bool didClose = false;
				StoryMapEventRewardView view = this.viewFactory.CreateRewardView();
				view.Initialize();
				view.ClickedClaim += delegate()
				{
					didClose = true;
				};
				UIViewManager.IUIViewStateGeneric<StoryMapEventRewardView> viewState = this.uiViewManager.ShowViewInstance<StoryMapEventRewardView>(view, new object[0]);
				while (!didClose)
				{
					yield return null;
				}
				yield return view.ClaimRewards(reward.Items);
				foreach (ItemAmount itemAmount in reward.Items)
				{
					this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "StoryEventReward");
				}
				storyState.SetLastClaimedReward((float)chapter);
				UserSettingsManager.Instance.SaveLocalSettings();
				view.Close(0);
				yield return viewState.WaitForClose();
			}
			yield break;
		}

		private IEnumerator ShowEndOfContent()
		{
			bool didClose = false;
			StoryMapEventEndOfContentView view = this.viewFactory.CreateEndOfContentView();
			view.ClickedContinue += delegate()
			{
				didClose = true;
			};
			UIViewManager.IUIViewStateGeneric<StoryMapEventEndOfContentView> viewState = this.uiViewManager.ShowViewInstance<StoryMapEventEndOfContentView>(view, new object[0]);
			while (!didClose)
			{
				yield return null;
			}
			view.Close(0);
			yield return viewState.WaitForClose();
			yield break;
		}

		private readonly IStoryManager storyManager;

		private readonly ConfigGetter<StoryMapEventConfig> configGetter;

		private readonly ViewFactory viewFactory;

		private readonly UIViewManager uiViewManager;

		private readonly InventoryManager inventoryManager;
	}
}
