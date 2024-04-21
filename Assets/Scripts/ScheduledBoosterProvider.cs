using System;
using TactileModules.PuzzleGame.ScheduledBooster;

public class ScheduledBoosterProvider : IScheduledBoosterProvider
{
	public bool IsTutorialLevel(ILevelProxy levelProxy)
	{
		LevelAsset levelAsset = levelProxy.LevelAsset as LevelAsset;
		return levelAsset == null || levelAsset.IsTutorial;
	}
}
