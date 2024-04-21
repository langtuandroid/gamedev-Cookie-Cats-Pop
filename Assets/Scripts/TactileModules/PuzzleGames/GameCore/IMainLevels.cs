using System;

namespace TactileModules.PuzzleGames.GameCore
{
	public interface IMainLevels
	{
		int GetFarthestUnlockedLevelIndex();

		int GetHumanNumberFromLevelIndex(int levelIndex);

		int MaxAvailableLevel { get; }
	}
}
