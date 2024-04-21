using System;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	public class BasicTaskEvent : BasicEvent
	{
		public BasicTaskEvent(MapTask mapTask, int day, int availableMainMapLevels)
		{
			this.TaskId = mapTask.ID;
			this.TaskName = mapTask.Title;
			this.StarCost = mapTask.StarsRequired;
			this.DayX = day;
			this.AvailableMainMapLevels = availableMainMapLevels;
			this.IsBuildTask = mapTask.IsBuildTask;
		}

		private TactileAnalytics.RequiredParam<string> TaskId { get; set; }

		private TactileAnalytics.RequiredParam<int> DayX { get; set; }

		private TactileAnalytics.RequiredParam<string> TaskName { get; set; }

		private TactileAnalytics.RequiredParam<int> StarCost { get; set; }

		private TactileAnalytics.RequiredParam<int> AvailableMainMapLevels { get; set; }

		private TactileAnalytics.RequiredParam<bool> IsBuildTask { get; set; }
	}
}
