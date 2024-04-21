using System;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace Shared.OneLifeChallenge
{
	public class ControllerFactory
	{
		public ControllerFactory(OneLifeChallengeManager oneLifeChallengeManager, IPlayFlowFactory playLevelSystem, MapFacade mapFacade, IFullScreenManager fullScreenManager, IFlowStack flowStack)
		{
			this.oneLifeChallengeManager = oneLifeChallengeManager;
			this.playLevelSystem = playLevelSystem;
			this.mapFacade = mapFacade;
			this.fullScreenManager = fullScreenManager;
			this.flowStack = flowStack;
		}

		public OneLifeChallengeMapFlow CreateMapFlow()
		{
			return new OneLifeChallengeMapFlow(this.oneLifeChallengeManager, this.playLevelSystem, this.mapFacade, this.fullScreenManager, this.flowStack);
		}

		private readonly OneLifeChallengeManager oneLifeChallengeManager;

		private readonly IPlayFlowFactory playLevelSystem;

		private readonly MapFacade mapFacade;

		private readonly IFullScreenManager fullScreenManager;

		private readonly IFlowStack flowStack;
	}
}
