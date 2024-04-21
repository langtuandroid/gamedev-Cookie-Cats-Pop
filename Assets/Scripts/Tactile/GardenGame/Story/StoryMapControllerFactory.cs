using System;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Story.Assets;
using TactileModules.GameCore.UI;

namespace Tactile.GardenGame.Story
{
	public class StoryMapControllerFactory : IStoryMapControllerFactory
	{
		public StoryMapControllerFactory(IUIController uiController, PropsManager propsManager, IStoryAudio storyAudio)
		{
			this.uiController = uiController;
			this.propsManager = propsManager;
			this.storyAudio = storyAudio;
			this.assets = new AssetModel();
		}

		public IStoryMapController Create(MainMapController mainMapController)
		{
			return new StoryMapController(this.uiController, mainMapController, this.assets, this.propsManager, this.storyAudio);
		}

		private readonly IUIController uiController;

		private readonly PropsManager propsManager;

		private readonly IAssetModel assets;

		private readonly IStoryAudio storyAudio;
	}
}
