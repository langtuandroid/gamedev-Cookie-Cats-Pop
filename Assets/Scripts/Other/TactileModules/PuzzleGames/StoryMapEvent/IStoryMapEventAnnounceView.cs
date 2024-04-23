using System;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public interface IStoryMapEventAnnounceView : IUIView
	{
		event Action CallToActionClicked;

		event Action DismissClicked;

		void SetTimeLeft(int seconds);
	}
}
