using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface ILevelRushNotificationProvider
	{
		string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
