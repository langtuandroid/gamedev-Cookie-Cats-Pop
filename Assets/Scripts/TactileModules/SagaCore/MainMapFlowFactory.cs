using System;
using TactileModules.Placements;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public class MainMapFlowFactory : IMainMapFlowFactory
	{
		public MainMapFlowFactory(IMainProgression mainProgressionManager, CloudClient cloudClient, FacebookClient facebookClient, MapFacade mapFacade, MapPopupManager mapPopupManager, IGameSessionManager gameSessionManager, IFullScreenManager fullScreen, IFlowStack flows, IStoryIntroProvider storyIntroProvider, IPlacementRunner placementRunner, IMainLevelsFlowFactory mainLevelsFlowFactory, LevelDatabaseCollection databaseCollection, MapStreamerCollection mapStreamerCollection)
		{
			this.mainProgressionManager = mainProgressionManager;
			this.cloudClient = cloudClient;
			this.facebookClient = facebookClient;
			this.mapFacade = mapFacade;
			this.mapPopupManager = mapPopupManager;
			this.gameSessionManager = gameSessionManager;
			this.fullScreen = fullScreen;
			this.flows = flows;
			this.storyIntroProvider = storyIntroProvider;
			this.placementRunner = placementRunner;
			this.mainLevelsFlowFactory = mainLevelsFlowFactory;
			this.databaseCollection = databaseCollection;
			this.mapStreamerCollection = mapStreamerCollection;
		}

		public MainMapFlow CreateMainMapFlow()
		{
			return new MainMapFlow(this.databaseCollection, this.mapStreamerCollection, this.cloudClient, this.facebookClient, this.mapFacade, this.mainProgressionManager, this.mapPopupManager, this.gameSessionManager, this.fullScreen, this.flows, this.placementRunner, this.mainLevelsFlowFactory, this.storyIntroProvider);
		}

		private readonly IMainProgression mainProgressionManager;

		private readonly CloudClient cloudClient;

		private readonly FacebookClient facebookClient;

		private readonly MapFacade mapFacade;

		private readonly MapPopupManager mapPopupManager;

		private readonly IGameSessionManager gameSessionManager;

		private readonly IFullScreenManager fullScreen;

		private readonly IFlowStack flows;

		private readonly IStoryIntroProvider storyIntroProvider;

		private readonly IPlacementRunner placementRunner;

		private readonly IMainLevelsFlowFactory mainLevelsFlowFactory;

		private readonly LevelDatabaseCollection databaseCollection;

		private readonly MapStreamerCollection mapStreamerCollection;
	}
}
