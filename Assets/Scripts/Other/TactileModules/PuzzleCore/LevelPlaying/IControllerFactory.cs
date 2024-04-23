using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IControllerFactory
	{
		LevelStartViewController CreateLevelStartViewController();

		ILevelSessionRunner CreateLevelSessionRunner(ICorePlayFlow corePlayFlow);

		ILevelAttempt CreateLevelAttempt(ICorePlayFlow corePlayFlow, ILevelSessionRunner sessionRunner);
	}
}
