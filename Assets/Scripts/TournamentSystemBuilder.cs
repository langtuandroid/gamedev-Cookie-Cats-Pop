using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.Lives;
using TactileModules.SagaCore;

public static class TournamentSystemBuilder
{
	public static ITournamentSystem Build(ISagaCoreSystem sagaCore, IPlayFlowFactory playFlowFactory, IFullScreenManager fullScreenManager, IFlowStack flowStack, CloudClient cloudClient, LevelDatabaseCollection levelDatabaseCollection, MapStreamerCollection mapStreamerCollection, TimeStampManager timeStampManager, TournamentManager.ITournamentUIProvider uiProvider, MapPopupManager mapPopupManager, ILivesManager livesManager)
	{
		TournamentCloudManager tournamentCloudManager = new TournamentCloudManager(cloudClient);
		TournamentManager.CreateInstance(uiProvider, tournamentCloudManager, levelDatabaseCollection, mapStreamerCollection, timeStampManager, livesManager);
		TournamentControllerFactory controllerFactory = new TournamentControllerFactory(cloudClient, sagaCore.MapFacade, playFlowFactory, fullScreenManager, flowStack, TournamentManager.Instance);
		mapPopupManager.RegisterPopupObject(new TournamentPopup(TournamentManager.Instance, controllerFactory));
		return new TournamentSystem(controllerFactory, tournamentCloudManager);
	}
}
