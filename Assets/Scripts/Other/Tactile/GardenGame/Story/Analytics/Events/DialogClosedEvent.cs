using System;

namespace Tactile.GardenGame.Story.Analytics.Events
{
	[TactileAnalytics.EventAttribute("dialogClosedEvent", true)]
	public class DialogClosedEvent : BasicEvent
	{
		public DialogClosedEvent(string taskId, string text, bool skipped, double secondsWaited, double totalDuration, string taskName)
		{
			this.TaskId = taskId;
			this.Text = text;
			this.Skipped = skipped;
			this.SecondsWaited = secondsWaited;
			this.TaskName = taskName;
			this.TotalDuration = totalDuration;
		}

		private TactileAnalytics.RequiredParam<string> TaskId { get; set; }

		private TactileAnalytics.RequiredParam<string> Text { get; set; }

		private TactileAnalytics.RequiredParam<string> TaskName { get; set; }

		private TactileAnalytics.RequiredParam<bool> Skipped { get; set; }

		private TactileAnalytics.RequiredParam<double> SecondsWaited { get; set; }

		private TactileAnalytics.RequiredParam<double> TotalDuration { get; set; }
	}
}
