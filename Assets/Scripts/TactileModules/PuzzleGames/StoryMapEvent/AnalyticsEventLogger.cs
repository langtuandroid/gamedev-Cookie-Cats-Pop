using System;
using Tactile;
using Tactile.GardenGame.Story;
using Tactile.GardenGame.Story.Dialog;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class AnalyticsEventLogger
	{
		public AnalyticsEventLogger(IAnalytics analytics, IStoryManager storyManager, InventoryManager inventoryManager)
		{
			this.analytics = analytics;
			this.storyManager = (storyManager as StoryManager);
			this.inventoryManager = inventoryManager;
			this.RegisterEvents();
		}

		private void RegisterEvents()
		{
			this.storyManager.TaskStarted += this.StoryManagerOnTaskStarted;
			this.storyManager.TaskEnded += this.StoryManagerOnTaskEnded;
			this.storyManager.TaskSkipped += this.StoryManagerOnTaskSkipped;
		}

		private void StoryManagerOnTaskStarted(MapTask task)
		{
			task.ActionEnded += this.TaskOnActionEnded;
			this.analytics.LogEvent(new TaskStartedEvent(task, this.storyManager.CurrentChapter, this.inventoryManager.GetAmount("Star")), -1.0, null);
			this.hasSkippedDialog = false;
		}

		private void StoryManagerOnTaskEnded(MapTask task)
		{
			task.ActionEnded -= this.TaskOnActionEnded;
			this.analytics.LogEvent(new TaskEndedEvent(task, this.storyManager.CurrentChapter, this.inventoryManager.GetAmount("Star"), this.hasSkippedDialog), -1.0, null);
		}

		private void TaskOnActionEnded(MapAction action, object result)
		{
			DialogOverlayResult dialogOverlayResult = result as DialogOverlayResult;
			if (dialogOverlayResult != null && dialogOverlayResult.WasSkipped)
			{
				this.hasSkippedDialog = true;
			}
		}

		private void StoryManagerOnTaskSkipped(MapTask task)
		{
			this.analytics.LogEvent(new TaskSkippedEvent(task, this.storyManager.CurrentChapter, this.inventoryManager.GetAmount("Star")), -1.0, null);
		}

		private readonly IAnalytics analytics;

		private readonly StoryManager storyManager;

		private readonly InventoryManager inventoryManager;

		private bool hasSkippedDialog;
	}
}
