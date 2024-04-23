using System;

namespace Tactile.GardenGame.Story
{
	public class StorySystem
	{
		public StorySystem(StoryManager storyManager, BrowseTasksFactory browseTasksFactory, IStoryControllerFactory storyControllerFactory)
		{
			this.StoryManager = storyManager;
			this.BrowseTasksFactory = browseTasksFactory;
			this.StoryControllerFactory = storyControllerFactory;
		}

		public StoryManager StoryManager { get; private set; }

		public BrowseTasksFactory BrowseTasksFactory { get; private set; }

		public IStoryControllerFactory StoryControllerFactory { get; private set; }
	}
}
