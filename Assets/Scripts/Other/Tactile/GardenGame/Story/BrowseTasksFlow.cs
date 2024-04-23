using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.Story.Assets;
using Tactile.GardenGame.Story.Rewards;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class BrowseTasksFlow : IFlow, IFiberRunnable
	{
		public BrowseTasksFlow(IUIController uiController, IStoryManager storyManager, IVisualInventory visualInventory, Action notEnoughStarsPlayClicked, IStoryRewardsFactory storyRewardsFactory, IAssetModel assets, TimedTaskModel timedTaskModel, FlowStack flowStack)
		{
			this.uiController = uiController;
			this.storyManager = storyManager;
			this.visualInventory = visualInventory;
			this.storyRewardsFactory = storyRewardsFactory;
			this.notEnoughStarsPlayClicked = notEnoughStarsPlayClicked;
			this.assets = assets;
			this.timedTaskModel = timedTaskModel;
			this.flowStack = flowStack;
			this.storyRewards = storyRewardsFactory.CreateStoryRewards();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnOutOfStoryContent;

		public IEnumerator Run()
		{
			Fiber completeTask = new Fiber(FiberBucket.Manual);
			EnumeratorResult<bool> wasPlayLevelClicked = new EnumeratorResult<bool>();
			bool hasTasks = this.storyManager.GetActiveTasks().Count > 0;
			bool hasClaimableReward = this.storyRewards.GetClaimableRewards().Count > 0;
			if (!hasTasks && !hasClaimableReward)
			{
				this.OutOfStoryContent();
				yield break;
			}
			this.browseTasksView = this.uiController.ShowView<BrowseTasksView>(this.assets.BrowseTasksView);
			this.browseTasksView.LockedCloseInput = true;
			List<StoryRewardIndicator> rewardIndicators = new List<StoryRewardIndicator>();
			List<StoryConfig.Reward> chapterRewards = this.storyRewards.GetVisibleChapterRewards();
			foreach (StoryConfig.Reward reward in chapterRewards)
			{
				StoryRewardIndicator storyRewardIndicator = UnityEngine.Object.Instantiate<StoryRewardIndicator>(this.assets.StoryRewardIndicator);
				storyRewardIndicator.Initialize(reward, this.visualInventory.InventoryManager, reward.RewardType);
				this.browseTasksView.AddRewardIndicator(storyRewardIndicator, reward.NormalizedProgression);
				rewardIndicators.Add(storyRewardIndicator);
			}
			this.browseTasksView.Initialize(this.storyManager, this.timedTaskModel);
			yield return this.browseTasksView.AnimateTaskCompleted();
			if (hasClaimableReward)
			{
				StoryRewardFlow c = this.storyRewardsFactory.CreateStoryRewardFlow();
				this.flowStack.Push(c);
				foreach (StoryRewardIndicator storyRewardIndicator2 in rewardIndicators)
				{
					if (storyRewardIndicator2.Reward.NormalizedProgression <= this.storyManager.GetChapterProgressionNormalized())
					{
						UnityEngine.Object.Destroy(storyRewardIndicator2.gameObject);
					}
				}
			}
			if (!hasTasks)
			{
				this.OutOfStoryContent();
				yield break;
			}
			this.browseTasksView.LockedCloseInput = false;
			this.browseTasksView.TaskClicked += delegate(MapTask task, Vector3 starTarget)
			{
				completeTask.Start(this.HandleTaskClick(task, this.browseTasksView, wasPlayLevelClicked, starTarget));
			};
			this.storyManager.ResetLastCompletedTask();
			while (this.browseTasksView.ClosingResult == null)
			{
				completeTask.Step();
				yield return null;
			}
			while (completeTask.Step())
			{
				yield return null;
			}
			if (wasPlayLevelClicked)
			{
				yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
				this.notEnoughStarsPlayClicked();
				this.storyManager.NotEnoughStarsPlayedWasClicked();
			}
			yield break;
		}

		private IEnumerator HandleTaskClick(MapTask task, BrowseTasksView view, EnumeratorResult<bool> wasPlayLevelClicked, Vector3 starTarget)
		{
			bool isTimedTask = this.storyManager.IsTaskTimerInProgress(task);
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.OnlyNegativeItemAmountFilter), isTimedTask ? null : view.TaskStarAnimator))
			{
				if (isTimedTask)
				{
					yield return this.storyManager.TryCompletingTimedTaskOrSkippingUsingCoins(task, this.AnimateInventoryAndCloseView(animator, view, starTarget));
				}
				else
				{
					yield return this.storyManager.TryCompletingTaskUsingStars(task, this.AnimateInventoryAndCloseView(animator, view, starTarget), this.ShowMessageForInsufficientStars(task, wasPlayLevelClicked), this.ShowTimedTaskView(task));
				}
			}
			yield break;
		}

		private IEnumerator AnimateInventoryAndCloseView(IInventoryItemAnimator animator, UIView view, Vector3 starTarget)
		{
			yield return animator.Animate(starTarget);
			view.Close(0);
			yield break;
		}

		private IEnumerator ShowMessageForInsufficientStars(MapTask task, EnumeratorResult<bool> wasPlayLevelClicked)
		{
			NotEnoughStarsView view = this.uiController.ShowView<NotEnoughStarsView>(this.assets.NotEnoughStarsView);
			while (view.ClosingResult == null)
			{
				yield return null;
			}
			int result = (int)view.ClosingResult;
			if (result == 1)
			{
				wasPlayLevelClicked.value = true;
				this.browseTasksView.Close(0);
			}
			yield break;
		}

		private IEnumerator ShowTimedTaskView(MapTask task)
		{
			TimedTaskView view = this.uiController.ShowView<TimedTaskView>(this.assets.TimedTaskView);
			view.Initialize(task, this.timedTaskModel.GetTimedTask(task));
			bool skipped = false;
			view.SkipClicked += delegate()
			{
				skipped = true;
			};
			while (view.ClosingResult == null)
			{
				if (skipped)
				{
					using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.OnlyNegativeItemAmountFilter)))
					{
						yield return this.storyManager.TryCompletingTimedTaskOrSkippingUsingCoins(task, this.AnimateInventoryAndCloseView(animator, view, view.CoinTarget.position));
					}
					skipped = false;
				}
				yield return null;
			}
			yield break;
		}

		private bool OnlyNegativeItemAmountFilter(InventoryItem item, int amount)
		{
			return amount < 0;
		}

		private void OutOfStoryContent()
		{
			if (this.OnOutOfStoryContent != null)
			{
				this.OnOutOfStoryContent();
			}
		}

		public void OnExit()
		{
		}

		private readonly IUIController uiController;

		private readonly IStoryManager storyManager;

		private readonly IVisualInventory visualInventory;

		private readonly IStoryRewardsFactory storyRewardsFactory;

		private readonly IStoryRewards storyRewards;

		private readonly IAssetModel assets;

		private readonly TimedTaskModel timedTaskModel;

		private readonly Action notEnoughStarsPlayClicked;

		private readonly FlowStack flowStack;

		private BrowseTasksView browseTasksView;
	}
}
