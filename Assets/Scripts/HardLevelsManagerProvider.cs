using System;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class HardLevelsManagerProvider : IHardLevelsProvider
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public LevelDatabase GetMainLevelDatabase()
	{
		return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
	}

	public int GetHumanNumber(int index)
	{
		LevelProxy levelProxy = new LevelProxy(this.GetMainLevelDatabase(), new int[]
		{
			index
		});
		return this.GetMainLevelDatabase().GetHumanNumber(levelProxy);
	}

	public DifficultyConfig Config()
	{
		return ConfigurationManager.Get<DifficultyConfig>();
	}
}
