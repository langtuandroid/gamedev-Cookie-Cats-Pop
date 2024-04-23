using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	[TactileAnalytics.EventAttribute("urlCachingStarted", true)]
	public class UrlCachingStartedEvent : EventBase
	{
		public UrlCachingStartedEvent(string url, string domain) : base(url, domain)
		{
		}
	}
}
