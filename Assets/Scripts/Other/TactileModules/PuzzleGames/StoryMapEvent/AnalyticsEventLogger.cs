using System;
using Tactile;
using Tactile.GardenGame.Story;
using Tactile.GardenGame.Story.Dialog;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class AnalyticsEventLogger
	{
		public AnalyticsEventLogger(IStoryManager storyManager, InventoryManager inventoryManager)
		{
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
			this.hasSkippedDialog = false;
		}

		private void StoryManagerOnTaskEnded(MapTask task)
		{
			task.ActionEnded -= this.TaskOnActionEnded;
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
			
		}

		private readonly StoryManager storyManager;

		private readonly InventoryManager inventoryManager;

		private bool hasSkippedDialog;
	}
}
