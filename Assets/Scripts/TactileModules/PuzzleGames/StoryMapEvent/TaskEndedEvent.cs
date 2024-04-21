using System;
using Tactile.GardenGame.Story;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	[TactileAnalytics.EventAttribute("storyMapTaskEnded", true)]
	public class TaskEndedEvent : BasicTaskEvent
	{
		public TaskEndedEvent(MapTask mapTask, int day, int availableMainMapLevels, bool dialogSkipped) : base(mapTask, day, availableMainMapLevels)
		{
			this.DialogSkipped = dialogSkipped;
		}

		private TactileAnalytics.RequiredParam<bool> DialogSkipped { get; set; }
	}
}
