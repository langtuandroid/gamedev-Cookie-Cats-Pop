using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayFlowFactory
	{
		ICorePlayFlow CreateCorePlayFlow(LevelProxy proxy, IPlayFlowContext playFlowContext);
	}
}
