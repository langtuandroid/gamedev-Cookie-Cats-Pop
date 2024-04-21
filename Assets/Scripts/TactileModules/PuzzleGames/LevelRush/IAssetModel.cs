using System;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface IAssetModel
	{
		LevelRushPresent LevelRushPresent { get; }

		LevelRushStartView LevelRushStartView { get; }

		LevelRushEndedView LevelRushEndedView { get; }

		LevelRushPresentInfoView LevelRushPresentInfoView { get; }

		LevelRushRewardView LevelRushRewardView { get; }
	}
}
