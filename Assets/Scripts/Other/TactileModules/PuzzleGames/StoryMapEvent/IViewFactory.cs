using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IViewFactory
	{
		IStoryMapEventAnnounceView CreateFeatureStartedView();

		IStoryMapEventAnnounceView CreateFeatureEndedView();

		IStoryMapEventAnnounceView CreateFeatureReminderView();

		StoryMapSideButton CreateSideButton();
	}
}
