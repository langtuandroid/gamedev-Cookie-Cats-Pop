using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IStoryMapEventNotificationProvider
	{
		string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData);
	}
}
