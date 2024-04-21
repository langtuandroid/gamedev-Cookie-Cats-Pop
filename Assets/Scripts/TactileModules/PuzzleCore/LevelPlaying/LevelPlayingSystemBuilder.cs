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

		public static void BuildAnalytics(IAnalytics analytics, string adjustMissionStartedEventId, IAdjustTracking adjustTracking, IMainProgressionForAnalytics mainProgression, IPlayFlowEvents playFlowEvents)
		{
			analytics.RegisterDecorator(new BasicEventDecorator(playFlowEvents));
			analytics.RegisterDecorator(new BasicMissionEventBaseDecorator(mainProgression));
			new LevelEventsLogger(analytics, playFlowEvents, adjustMissionStartedEventId, adjustTracking, mainProgression);
		}
	}
}
