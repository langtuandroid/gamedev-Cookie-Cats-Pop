using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Model
{
	public interface ISlidesAndLaddersFeatureProgression
	{
		ResultState ResultState { get; }

		int CurrentLevelIndex { get; set; }

		int FarthestUnlockedLevelIndex { get; set; }

		bool HasShownTutorial { get; set; }

		bool CompletedFeature { get; set; }

		void SetResultState(ResultState state);

		bool IsReadyToPlayLevel(ILevelProxy level);

		bool IsLevelIndexEndChest(int levelIndex);

		int FeatureSpinCount();
	}
}
