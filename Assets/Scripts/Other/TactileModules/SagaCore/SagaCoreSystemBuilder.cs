using System;
using Shared.SagaCore.Module.MainLevels;
using Shared.SagaCore.Module.MainLevels.Analytics;
using Tactile.SagaCore.MainLevels;
using TactileModules.Analytics.Interfaces;
using TactileModules.Placements;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	public static class SagaCoreSystemBuilder
	{
		public static ISagaCoreSystem Build(IFlowStack flowStack, IFullScreenManager fullScreenManager, IPlayFlowFactory playFlowFactory, MainProgressionManager mainProgressionManager, LeaderboardManager leaderBoardManager, GateManager gateManager, CloudClient cloudClient, MapPopupManager mapPopupManager, IGameSessionManager gameSessionManager, IStoryIntroProvider storyIntroProvider, IPlacementRunner placementRunner, IAnalytics analytics, LevelDatabaseCollection databaseCollection, MapStreamerCollection mapStreamerCollection, HardLevelsManager hardLevelsManager)
		{
			MapFacade mapFacade = new MapFacade(cloudClient);
			MainLevelsFlowFactory mainLevelsFlowFactory = new MainLevelsFlowFactory(playFlowFactory, mainProgressionManager, leaderBoardManager, gateManager);
			mapFacade.MapPlugins.Add(new HardLevelsMapRefresher(hardLevelsManager));
			MainMapFlowFactory mainMapFlowFactory = new MainMapFlowFactory(mainProgressionManager, cloudClient, mapFacade, mapPopupManager, gameSessionManager, fullScreenManager, flowStack, storyIntroProvider, placementRunner, mainLevelsFlowFactory, databaseCollection, mapStreamerCollection);
			Tactile.SagaCore.MainLevels.BasicEventDecorator decorator = new Tactile.SagaCore.MainLevels.BasicEventDecorator(mainProgressionManager, gateManager);
			analytics.RegisterDecorator(decorator);
			Shared.SagaCore.Module.MainLevels.Analytics.BasicMissionEventBaseDecorator decorator2 = new Shared.SagaCore.Module.MainLevels.Analytics.BasicMissionEventBaseDecorator(gateManager);
			analytics.RegisterDecorator(decorator2);
			return new SagaCoreSystem(mainMapFlowFactory, mapFacade);
		}
	}
}
