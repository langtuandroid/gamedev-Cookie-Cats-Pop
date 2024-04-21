using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.TreasureHunt
{
	public interface ITreasureHuntProvider
	{
		ILevelAccomplishment NewLevelAccomplishment();

		string GetNotificationText(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
