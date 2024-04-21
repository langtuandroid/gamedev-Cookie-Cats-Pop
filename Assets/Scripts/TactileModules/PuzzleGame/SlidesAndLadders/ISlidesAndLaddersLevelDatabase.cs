using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.SlidesAndLadders
{
	public interface ISlidesAndLaddersLevelDatabase
	{
		List<LevelConnection> LevelConnections { get; }

		int NumberOfLevels { get; }

		ILevelProxy NextLevel(ILevelProxy current, bool isLevelCompleted);

		ILevelProxy GetLevelProxy(int levelIndex);

		bool IsTreasureLevel(int index);

		bool IsSlideLevel(int index);

		bool IsLadderLevel(int index);

		bool IsNeitherSlideOrLadderLevel(int index);

		int GetChestRank(int index);
	}
}
