using System;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushControllerFactory
	{
		public LevelRushControllerFactory(ILevelRushActivation levelRushActivation, IMainMapPlugin mainMapPlugin, IAssetModel assetModel)
		{
			this.levelRushActivation = levelRushActivation;
			this.mainMapPlugin = mainMapPlugin;
			this.assetModel = assetModel;
		}

		public ActivateAndShowLocalLevelRushRunner CreateStartLevelRushRunner()
		{
			return new ActivateAndShowLocalLevelRushRunner(this.levelRushActivation, this.mainMapPlugin, this.assetModel);
		}

		private readonly ILevelRushActivation levelRushActivation;

		private readonly IMainMapPlugin mainMapPlugin;

		private readonly IAssetModel assetModel;
	}
}
