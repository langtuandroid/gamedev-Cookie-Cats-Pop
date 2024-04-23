using System;

namespace TactileModules.UrlCaching.Analytics.Events
{
	[TactileAnalytics.EventAttribute("urlCachingCompleted", true)]
	public class UrlCachingCompletedEvent : EventBase
	{
		public UrlCachingCompletedEvent(string url, string domain, int cachedFileSize) : base(url, domain)
		{
			this.CachedFileSize = cachedFileSize;
		}

		private TactileAnalytics.RequiredParam<int> CachedFileSize { get; set; }
	}
}
