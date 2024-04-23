using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayFlowEvents
	{
		event Action<ICorePlayFlow> PlayFlowCreated;
	}
}
