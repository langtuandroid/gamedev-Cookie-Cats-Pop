using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGames.TreasureHunt;

public class TreasureHuntManagerProvider : ITreasureHuntProvider
{
	public string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return string.Format(L.Get("There is only {0} hours left of the Treasure Hunt!"), timeSpan.TotalHours);
	}

	public ILevelAccomplishment NewLevelAccomplishment()
	{
		return new LevelAccomplishment();
	}
}
