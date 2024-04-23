using System;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface IMainLevelsIndices
	{
		int GetHumanNumberFromLevelIndex(int levelIndex);

		int GetMaxAvailableLevelIndex();
	}
}
