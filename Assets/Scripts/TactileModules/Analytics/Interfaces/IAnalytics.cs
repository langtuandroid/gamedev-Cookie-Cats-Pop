using System;

namespace TactileModules.Analytics.Interfaces
{
	public interface IAnalytics
	{
		void RegisterDecorator(IEventDecorator decorator);

		void LogEvent(object eventObject, double overrideEventTimestamp = -1.0, string overrideSessionId = null);
	}
}
