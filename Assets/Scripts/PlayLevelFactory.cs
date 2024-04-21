using System;
using TactileModules.PuzzleCore.LevelPlaying;

public class PlayLevelFactory : IPlayLevelImplementationFactory
{
	public IPlayLevel CreatePlayLevelImplementor()
	{
		return new PlayLevel();
	}
}
