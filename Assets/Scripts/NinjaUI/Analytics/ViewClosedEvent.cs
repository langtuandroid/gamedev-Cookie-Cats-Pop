using System;

namespace NinjaUI.Analytics
{
	[TactileAnalytics.EventAttribute("viewClosed", true)]
	public class ViewClosedEvent : BasicEvent
	{
		public ViewClosedEvent(string viewName, string viewButton, string viewParams)
		{
			this.ViewName = viewName;
			this.ViewButton = viewButton;
			this.ViewParams = viewParams;
		}

		public TactileAnalytics.RequiredParam<string> ViewName { get; set; }

		public TactileAnalytics.RequiredParam<string> ViewButton { get; set; }

		public TactileAnalytics.OptionalParam<string> ViewParams { get; set; }
	}
}
