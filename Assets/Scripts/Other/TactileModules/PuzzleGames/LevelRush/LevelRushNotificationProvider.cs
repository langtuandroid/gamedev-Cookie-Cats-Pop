using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelRush
{
	public class LevelRushNotificationProvider : ILevelRushNotificationProvider
	{
		public string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
		{
			return NotificationManager.Instance.TryAddGreetingWithName(string.Format(L.Get("Time to get going! You only have {0} hours left to complete the Level Rush!"), timeSpan.TotalHours));
		}
	}
}
