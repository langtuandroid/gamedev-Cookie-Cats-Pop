using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface ICurrencyAvailability
	{
		bool HasMainProgressionLevelsToPlay { get; }

		int MinimumLevelIndex { get; }

		int GetRandomEndOfContentLevelIndex();

		bool ShouldLevelAwardStoryCurrency(int levelIndex);
	}
}
