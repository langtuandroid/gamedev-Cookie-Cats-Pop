using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	[TactileAnalytics.EventAttribute("urlCachingWriteAllBytesStarted", true)]
	public class UrlCachingWriteAllBytesStartedEvent : EventBase
	{
		public UrlCachingWriteAllBytesStartedEvent(string url, string domain) : base(url, domain)
		{
		}
	}
}
