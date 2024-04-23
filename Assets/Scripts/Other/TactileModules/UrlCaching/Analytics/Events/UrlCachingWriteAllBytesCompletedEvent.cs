using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	[TactileAnalytics.EventAttribute("urlCachingWriteAllBytesCompleted", true)]
	public class UrlCachingWriteAllBytesCompletedEvent : EventBase
	{
		public UrlCachingWriteAllBytesCompletedEvent(string url, string domain) : base(url, domain)
		{
		}
	}
}
