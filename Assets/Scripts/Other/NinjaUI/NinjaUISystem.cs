using System;
using NinjaUI.Analytics;

namespace NinjaUI
{
	public class NinjaUISystem
	{
		public NinjaUISystem(IUIViewManager viewManager, UIAnalyticsEventLogger analyticsEventLogger)
		{
			this.ViewManager = viewManager;
			this.AnalyticsEventLogger = analyticsEventLogger;
		}

		public IUIViewManager ViewManager { get; private set; }

		public UIAnalyticsEventLogger AnalyticsEventLogger { get; private set; }
	}
}
