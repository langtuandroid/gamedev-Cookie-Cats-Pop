using System;
using TactileModules.PuzzleGames.Lives;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public class ControllerFactory : IControllerFactory
	{
		public ControllerFactory(IAssetFactory assetFactory, ILivesManager livesManager, IViewPresenter viewPresenter)
		{
			this.assetFactory = assetFactory;
			this.livesManager = livesManager;
			this.viewPresenter = viewPresenter;
		}

		public LevelStartViewController CreateLevelStartViewController()
		{
			return new LevelStartViewController(this.assetFactory, this.viewPresenter);
		}

		public ILevelSessionRunner CreateLevelSessionRunner(ICorePlayFlow corePlayFlow)
		{
			return new LevelSessionRunner(corePlayFlow, this.livesManager, this);
		}

		public ILevelAttempt CreateLevelAttempt(ICorePlayFlow corePlayFlow, ILevelSessionRunner sessionRunner)
		{
			return new LevelAttempt(corePlayFlow.LevelProxy, corePlayFlow.GameImplementation, corePlayFlow, sessionRunner);
		}

		private readonly IAssetFactory assetFactory;

		private readonly ILivesManager livesManager;

		private readonly IViewPresenter viewPresenter;
	}
}
