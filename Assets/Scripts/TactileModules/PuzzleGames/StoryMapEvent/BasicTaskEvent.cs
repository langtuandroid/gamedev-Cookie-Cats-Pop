using System;
using Tactile.GardenGame.Story;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public abstract class BasicTaskEvent : BasicEvent
	{
		protected BasicTaskEvent(MapTask mapTask, int day, int pageTotalAmount)
		{
			this.TaskId = mapTask.ID;
			this.TaskName = mapTask.Title;
			this.PageCost = mapTask.StarsRequired;
			this.DayX = day;
			this.PageTotalAmount = pageTotalAmount;
		}

		private TactileAnalytics.RequiredParam<string> TaskId { get; set; }

		private TactileAnalytics.RequiredParam<int> DayX { get; set; }

		private TactileAnalytics.RequiredParam<string> TaskName { get; set; }

		private TactileAnalytics.RequiredParam<int> PageCost { get; set; }

		private TactileAnalytics.RequiredParam<int> PageTotalAmount { get; set; }
	}
}
