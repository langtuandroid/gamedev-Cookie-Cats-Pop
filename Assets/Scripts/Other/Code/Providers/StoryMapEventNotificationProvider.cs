using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.PuzzleGames.StoryMapEvent;

namespace Code.Providers
{
	public class StoryMapEventNotificationProvider : IStoryMapEventNotificationProvider
	{
		public string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
		{
			return NotificationManager.Instance.TryAddGreetingWithName(string.Format(L.Get("Time to get going! You only have {0} hours left to complete the "), timeSpan.TotalHours));
		}
	}
}
