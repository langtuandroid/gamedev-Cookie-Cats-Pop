using System;

namespace TactileModules.PuzzleGames.LevelDash
{
	public interface ILevelDashSystem
	{
		LevelDashManager LevelDashManager { get; }

		LevelDashViewController LevelDashViewController { get; }
	}
}
