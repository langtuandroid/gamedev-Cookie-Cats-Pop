using System;
using System.Diagnostics;
using Tactile.GardenGame.Story;
using TactileModules.GameCore.Boot;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.Placements;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.MapSystem
{
	public class MainMapStateFactory
	{
		public MainMapStateFactory(IUIController uiController, IFullScreenManager fullScreenManager, IStoryControllerFactory storyControllerFactory, PropsManager propsManager, StoryManager storyManager, IPlacementRunner placementRunner, IUserSettings userSettings, FlowStack flowStack)
		{
			this.uiController = uiController;
			this.fullScreenManager = fullScreenManager;
			this.storyManager = storyManager;
			this.placementRunner = placementRunner;
			this.storyControllerFactory = storyControllerFactory;
			this.propsManager = propsManager;
			this.flowStack = flowStack;
			this.userSettings = userSettings;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MainMapState> StateCreated;



		public MainMapState CreateState(IMainMapStateProvider mainMapStateProvider)
		{
			MainMapState mainMapState = new MainMapState(this.uiController, this.fullScreenManager, this.storyControllerFactory, this.storyManager, this.propsManager, new AssetModel(), this.placementRunner, this.userSettings, this.flowStack, mainMapStateProvider);
			if (this.StateCreated != null)
			{
				this.StateCreated(mainMapState);
			}
			return mainMapState;
		}

		private readonly IUIController uiController;

		private readonly IFullScreenManager fullScreenManager;

		private readonly StoryManager storyManager;

		private readonly IPlacementRunner placementRunner;

		private readonly IStoryControllerFactory storyControllerFactory;

		private readonly PropsManager propsManager;

		private readonly FlowStack flowStack;

		private readonly IUserSettings userSettings;
	}
}
