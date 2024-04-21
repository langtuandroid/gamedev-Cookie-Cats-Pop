using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IAssetFactory
	{
		ILevelStartView CreateLevelStartView();
	}
}
