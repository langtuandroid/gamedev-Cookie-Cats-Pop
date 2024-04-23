using System;
using Tactile.GardenGame.Story;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	[TactileAnalytics.EventAttribute("storyMapTaskStarted", true)]
	public class TaskStartedEvent : BasicTaskEvent
	{
		public TaskStartedEvent(MapTask mapTask, int day, int pageTotalAmount) : base(mapTask, day, pageTotalAmount)
		{
		}
	}
}
