using System;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	[TactileAnalytics.EventAttribute("taskStartedEvent", true)]
	public class TaskStartedEvent : BasicTaskEvent
	{
		public TaskStartedEvent(MapTask mapTask, int day, int availableMainMapLevels) : base(mapTask, day, availableMainMapLevels)
		{
		}
	}
}
