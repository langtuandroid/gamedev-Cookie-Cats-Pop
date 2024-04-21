using System;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	[TactileAnalytics.EventAttribute("taskEndedEvent", true)]
	public class TaskEndedEvent : BasicTaskEvent
	{
		public TaskEndedEvent(MapTask mapTask, int day, int availableMainMapLevels, bool skipped) : base(mapTask, day, availableMainMapLevels)
		{
			this.Skipped = skipped;
		}

		private TactileAnalytics.RequiredParam<bool> Skipped { get; set; }
	}
}
