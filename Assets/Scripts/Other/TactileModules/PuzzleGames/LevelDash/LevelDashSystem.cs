using System;

namespace TactileModules.PuzzleGames.LevelDash
{
	public class LevelDashSystem : ILevelDashSystem
	{
		public LevelDashSystem(LevelDashManager manager, LevelDashViewController viewController)
		{
			this.LevelDashManager = manager;
			this.LevelDashViewController = viewController;
		}

		public LevelDashManager LevelDashManager { get; private set; }

		public LevelDashViewController LevelDashViewController { get; private set; }
	}
}
