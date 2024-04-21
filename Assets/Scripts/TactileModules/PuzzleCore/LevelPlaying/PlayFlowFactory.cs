using System;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class PlayFlowFactory : IPlayFlowFactory
	{
		public PlayFlowFactory(IFullScreenManager fullScreenManager, ControllerFactory controllerFactory, IPlayLevelImplementationFactory playLevelImplementationFactory, IPlayFlowEventEmitter playFlowEventEmitter)
		{
			this.fullScreenManager = fullScreenManager;
			this.controllerFactory = controllerFactory;
			this.playLevelImplementationFactory = playLevelImplementationFactory;
			this.playFlowEventEmitter = playFlowEventEmitter;
		}

		public ICorePlayFlow CreateCorePlayFlow(LevelProxy proxy, IPlayFlowContext playFlowContext)
		{
			CorePlayFlow corePlayFlow = new CorePlayFlow(this.fullScreenManager, this.playLevelImplementationFactory, proxy, playFlowContext, this.controllerFactory);
			this.playFlowEventEmitter.EmitPlayFlowCreated(corePlayFlow);
			return corePlayFlow;
		}

		private readonly IFullScreenManager fullScreenManager;

		private readonly ControllerFactory controllerFactory;

		private readonly IPlayLevelImplementationFactory playLevelImplementationFactory;

		private readonly IPlayFlowEventEmitter playFlowEventEmitter;
	}
}
