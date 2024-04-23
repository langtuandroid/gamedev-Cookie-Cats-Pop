using System;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IMainProgressionForAnalytics
	{
		int MaxAvailableLevel { get; }

		int MaxAvailableLevelHumanNumber { get; }

		LevelProxy GetFarthestCompletedLevelProxy();

		int GetFarthestCompletedLevelIndex();

		int GetFarthestUnlockedLevelIndex();

		int GetFarthestCompletedLevelHumanNumber();

		int GetFarthestUnlockedLevelHumanNumber();

		LevelProxy GetFarthestUnlockedLevelProxy();
	}
}
