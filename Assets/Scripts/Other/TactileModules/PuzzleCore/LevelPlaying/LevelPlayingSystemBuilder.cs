using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.PuzzleGames.GameCore.Analytics;
using TactileModules.PuzzleGames.Lives;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public static class LevelPlayingSystemBuilder
	{
		public static LevelPlayingSystem Build(ILivesManager livesManager, IFullScreenManager fullScreenManager, IPlayLevelImplementationFactory playLevelImplementationFactory, IViewPresenter viewPresenter)
		{
			AssetFactory assetFactory = new AssetFactory();
			ControllerFactory controllerFactory = new ControllerFactory(assetFactory, livesManager, viewPresenter);
			PlayFlowEvents playFlowEvents = new PlayFlowEvents();
			PlayFlowFactory playFlowFactory = new PlayFlowFactory(fullScreenManager, controllerFactory, playLevelImplementationFactory, playFlowEvents);
			return new LevelPlayingSystem(playFlowFactory, playFlowEvents);
		}

		public static void BuildAnalytics(string adjustMissionStartedEventId, IAdjustTracking adjustTracking, IMainProgressionForAnalytics mainProgression, IPlayFlowEvents playFlowEvents)
		{
			new LevelEventsLogger(playFlowEvents, adjustMissionStartedEventId, adjustTracking, mainProgression);
		}
	}
}
