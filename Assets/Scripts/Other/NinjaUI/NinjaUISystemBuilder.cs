using System;
using NinjaUI.Analytics;
using TactileModules.Analytics.Interfaces;

namespace NinjaUI
{
	public class NinjaUISystemBuilder
	{
		public static NinjaUISystem Build()
		{
			UIViewManager instance = UIViewManager.Instance;
			UIAnalyticsEventLogger analyticsEventLogger = new UIAnalyticsEventLogger(instance);
			return new NinjaUISystem(instance, analyticsEventLogger);
		}
	}
}
