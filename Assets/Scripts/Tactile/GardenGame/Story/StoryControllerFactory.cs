using System;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public class StoryControllerFactory : IStoryControllerFactory
	{
		public StoryControllerFactory(StoryManager storyManager, IStoryMapControllerFactory storyMapControllerFactory, PropsManager propsManager)
		{
			this.storyManager = storyManager;
			this.storyMapControllerFactory = storyMapControllerFactory;
			this.propsManager = propsManager;
		}

		public IStoryController CreateController(MainMapController mainMapController)
		{
			return new StoryController(this.storyManager, this.storyMapControllerFactory, this.propsManager, mainMapController);
		}

		private readonly StoryManager storyManager;

		private readonly IStoryMapControllerFactory storyMapControllerFactory;

		private readonly PropsManager propsManager;
	}
}
