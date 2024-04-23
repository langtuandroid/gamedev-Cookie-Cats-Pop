using System;

namespace TactileModules.PuzzleGame.MainLevels
{
	public interface IMainProgression
	{
		event Action DeveloperCheated;

		LevelProxy GetFarthestCompletedLevelProxy();

		int GetFarthestCompletedLevelIndex();

		int GetFarthestUnlockedLevelIndex();

		int GetFarthestCompletedLevelHumanNumber();

		int GetFarthestUnlockedLevelHumanNumber();

		LevelProxy GetFarthestUnlockedLevelProxy();

		int MaxAvailableLevel { get; }

		MainLevelDatabase GetDatabase();

		void Developer_CompleteLevels(int upToLevelId, int wantedStars = 3, bool notifyProvider = true);

		int GetMaxAvailableLevelHumanNumber();

		int GetNumberOfStarsInLevel(int levelIndex);
	}
}
