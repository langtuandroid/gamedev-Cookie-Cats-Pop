using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayFlowEventEmitter
	{
		void EmitPlayFlowCreated(ICorePlayFlow corePlayFlow);
	}
}
