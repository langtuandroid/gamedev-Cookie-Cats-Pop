using System;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class LevelReleaseManagerProvider : LevelReleaseManager.ILevelReleaseManagerProvider
{
	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public int GetHumanNumber(int index)
	{
		MainLevelDatabase levelDatabase = this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		LevelProxy levelProxy = new LevelProxy(levelDatabase, new int[]
		{
			index
		});
		return levelDatabase.GetHumanNumber(levelProxy);
	}
}
