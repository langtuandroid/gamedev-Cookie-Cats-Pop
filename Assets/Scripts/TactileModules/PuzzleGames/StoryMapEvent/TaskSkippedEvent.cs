using System;
using Tactile.GardenGame.Story;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	[TactileAnalytics.EventAttribute("storyMapTaskSkipped", true)]
	public class TaskSkippedEvent : BasicTaskEvent
	{
		public TaskSkippedEvent(MapTask mapTask, int day, int pageTotalAmount) : base(mapTask, day, pageTotalAmount)
		{
		}
	}
}
