using System;

namespace NinjaUI.Analytics
{
	[TactileAnalytics.EventAttribute("viewShown", true)]
	public class ViewShownEvent : BasicEvent
	{
		public ViewShownEvent(string viewName, string viewParams)
		{
			this.ViewName = viewName;
			this.ViewParams = viewParams;
		}

		public TactileAnalytics.RequiredParam<string> ViewName { get; set; }

		public TactileAnalytics.OptionalParam<string> ViewParams { get; set; }
	}
}
