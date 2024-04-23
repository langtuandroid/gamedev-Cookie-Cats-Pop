using System;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	[TactileAnalytics.EventAttribute("taskSkippedEvent", true)]
	public class TaskSkippedEvent : BasicTaskEvent
	{
		public TaskSkippedEvent(MapTask mapTask, int day, int availableMainMapLevels, string skipTo) : base(mapTask, day, availableMainMapLevels)
		{
			this.SkipTo = skipTo;
		}

		private TactileAnalytics.RequiredParam<string> SkipTo { get; set; }
	}
}
