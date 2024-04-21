using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayLevelImplementationFactory
	{
		IPlayLevel CreatePlayLevelImplementor();
	}
}
