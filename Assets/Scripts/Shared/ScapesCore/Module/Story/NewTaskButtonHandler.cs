using System;
using System.Collections.Generic;
using Tactile.GameCore.Settings.Buttons;
using Tactile.GardenGame.Story;
using Tactile.GardenGame.Story.Assets;
using TactileModules.GameCore.ButtonArea;
using TactileModules.PuzzleGames.GameCore;

namespace Shared.ScapesCore.Module.Story
{
	public class NewTaskButtonHandler : ButtonHandler
	{
		public NewTaskButtonHandler(IButtonAreaModel buttonAreaModel, IAssetModel assets, StoryManager storyManager, BrowseTasksFactory browseTasksFactory, FlowStack flowStack) : base(buttonAreaModel, assets.NewTaskBubble)
		{
			this.storyManager = storyManager;
			this.browseTasksFactory = browseTasksFactory;
			this.flowStack = flowStack;
			if (storyManager.NoTasksCompleted)
			{
				this.dontFadeBubbleAwayNextTime = true;
			}
			storyManager.NewTaskUnlocked += this.StoryManagerOnNewTaskUnlocked;
		}

		protected override void HandleButtonCreated(ButtonAreaButton button)
		{
			this.newTaskAnimator = button.gameObject.GetComponent<NewTaskAnimator>();
			this.newTaskAnimator.Enabled = false;
			this.storyManager.BrowseTasksStarted += this.StoryManagerOnBrowseTasksStarted;
			this.storyManager.TaskEnded += this.StoryManagerOnTaskEnded;
			if (this.storyManager.IsFirstTaskActive)
			{
				this.ShowCurrentTask();
			}
		}

		protected override void HandleButtonDestroyed(ButtonAreaButton button)
		{
			this.newTaskAnimator = null;
			this.storyManager.BrowseTasksStarted -= this.StoryManagerOnBrowseTasksStarted;
			this.storyManager.TaskEnded -= this.StoryManagerOnTaskEnded;
		}

		protected override void Clicked()
		{
			BrowseTasksFlow c = this.browseTasksFactory.CreateBrowseTasksFlow(new Action(this.storyManager.NotEnoughStarsPlayedWasClicked));
			this.flowStack.Push(c);
			this.storyManager.BrowseTaskStart();
		}

		private void ShowCurrentTask()
		{
			this.dontFadeBubbleAwayNextTime = true;
			this.StoryManagerOnNewTaskUnlocked(this.storyManager.GetActiveTasks()[0]);
			this.StoryManagerOnTaskEnded(this.storyManager.GetActiveTasks()[0]);
		}

		private void StoryManagerOnBrowseTasksStarted()
		{
			this.newTaskAnimator.Enabled = false;
		}

		private void StoryManagerOnTaskEnded(MapTask obj)
		{
			if (this.unlockedTasks.Count == 0)
			{
				return;
			}
			if (this.dontFadeBubbleAwayNextTime)
			{
				this.newTaskAnimator.Enabled = true;
				this.newTaskAnimator.DisableAnimationPivot();
			}
			this.newTaskAnimator.StartAnimatingNewTasks(this.unlockedTasks, 0.3f, this.dontFadeBubbleAwayNextTime);
			this.unlockedTasks.Clear();
			this.dontFadeBubbleAwayNextTime = false;
		}

		private void StoryManagerOnNewTaskUnlocked(MapTask mapTask)
		{
			this.unlockedTasks.Add(mapTask);
		}

		private readonly StoryManager storyManager;

		private readonly BrowseTasksFactory browseTasksFactory;

		private readonly FlowStack flowStack;

		private NewTaskAnimator newTaskAnimator;

		private readonly List<MapTask> unlockedTasks = new List<MapTask>();

		private bool dontFadeBubbleAwayNextTime;
	}
}
