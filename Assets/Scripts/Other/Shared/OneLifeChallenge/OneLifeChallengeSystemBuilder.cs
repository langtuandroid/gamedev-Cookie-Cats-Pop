using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace Shared.OneLifeChallenge
{
	public static class OneLifeChallengeSystemBuilder
	{
		public static IOneLifeChallengeSystem Build(OneLifeChallengeManager oneLifeChallengeManager, ISagaCoreSystem sagaCore, IPlayFlowFactory playFlowFactory, IFullScreenManager fullScreenManager, IFlowStack flowStack)
		{
			ControllerFactory controllerFactory = new ControllerFactory(oneLifeChallengeManager, playFlowFactory, sagaCore.MapFacade, fullScreenManager, flowStack);
			return new OneLifeChallengeSystem(oneLifeChallengeManager, controllerFactory);
		}
	}
}
