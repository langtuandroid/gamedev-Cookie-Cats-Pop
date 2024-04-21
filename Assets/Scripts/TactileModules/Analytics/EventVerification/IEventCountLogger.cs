using System;

namespace TactileModules.Analytics.EventVerification
{
	public interface IEventCountLogger
	{
		void LogEvent(string schemaHash, string eventName, double unixTimestamp);
	}
}
