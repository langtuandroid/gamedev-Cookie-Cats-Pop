using System;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class MainLevelsIndices : IMainLevelsIndices
	{
		public MainLevelsIndices(MainProgressionManager mainProgressionManager)
		{
			this.mainProgressionManager = mainProgressionManager;
		}

		public int GetMaxAvailableLevelIndex()
		{
			return this.mainProgressionManager.GetDatabase().NumberOfAvailableLevels;
		}

		public int GetHumanNumberFromLevelIndex(int levelIndex)
		{
			MainLevelDatabase database = this.mainProgressionManager.GetDatabase();
			return database.GetHumanNumber(new LevelProxy(database, new int[]
			{
				levelIndex
			}));
		}

		private readonly MainProgressionManager mainProgressionManager;
	}
}
