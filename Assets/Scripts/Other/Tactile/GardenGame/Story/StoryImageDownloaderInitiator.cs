using System;
using System.Collections.Generic;

namespace Tactile.GardenGame.Story
{
	public class StoryImageDownloaderInitiator
	{
		public StoryImageDownloaderInitiator(StoryManager storyManager)
		{
			this.storyManager = storyManager;
			this.storyManager.NewTasksUnlocked += this.InitiateDownloadOfImagesInActiveTasks;
			this.InitiateDownloadOfImagesInActiveTasks();
		}

		private void InitiateDownloadOfImagesInActiveTasks()
		{
			List<MapTask> activeTasks = this.storyManager.GetActiveTasks();
			foreach (MapTask task in activeTasks)
			{
				this.StartImageDownloadsInTask(task);
			}
		}

		private void StartImageDownloadsInTask(MapTask task)
		{
			this.StartImageDownloadsInImageActions(task.GetActionsOfType<MapActionShowImage>(MapAction.ActionType.Default));
			this.StartImageDownloadsInImageActions(task.GetActionsOfType<MapActionShowImage>(MapAction.ActionType.Intro));
		}

		private void StartImageDownloadsInImageActions(List<MapActionShowImage> actions)
		{
			if (actions == null || actions.Count == 0)
			{
				return;
			}
			foreach (MapActionShowImage mapActionShowImage in actions)
			{
				mapActionShowImage.image.StartDownload();
			}
		}

		private readonly StoryManager storyManager;
	}
}
