using System;
using NinjaUI.Analytics;
using TactileModules.Analytics.Interfaces;

namespace NinjaUI
{
	public class NinjaUISystemBuilder
	{
		public static NinjaUISystem Build(IAnalytics analytics)
		{
			UIViewManager instance = UIViewManager.Instance;
			UIAnalyticsEventLogger analyticsEventLogger = new UIAnalyticsEventLogger(analytics, instance);
			return new NinjaUISystem(instance, analyticsEventLogger);
		}
	}
}
