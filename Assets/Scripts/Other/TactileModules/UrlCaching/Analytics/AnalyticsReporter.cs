using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.UrlCaching.Analytics.Events;

namespace TactileModules.UrlCaching.Analytics
{
	public class AnalyticsReporter : IAnalyticsReporter
	{
		public AnalyticsReporter(IAnalytics analytics, string domain)
		{
			this.analytics = analytics;
			this.domain = domain;
		}

		public void ReportUrlCachingStarted(string url)
		{
			this.analytics.LogEvent(new UrlCachingStartedEvent(url, this.domain), -1.0, null);
		}

		public void ReportUrlCachingCompleted(string url, int cachedFileSize)
		{
			this.analytics.LogEvent(new UrlCachingCompletedEvent(url, this.domain, cachedFileSize), -1.0, null);
		}

		public void ReportUrlCachingWriteAllBytesStarted(string url)
		{
			this.analytics.LogEvent(new UrlCachingWriteAllBytesStartedEvent(url, this.domain), -1.0, null);
		}

		public void ReportUrlCachingWriteAllBytesCompleted(string url)
		{
			this.analytics.LogEvent(new UrlCachingWriteAllBytesCompletedEvent(url, this.domain), -1.0, null);
		}

		public void ReportUrlCachingError(string url, string errorName, string errorMessage, Exception exception)
		{
			this.analytics.LogEvent(new UrlCachingErrorEvent(url, this.domain, errorName, errorMessage, exception), -1.0, null);
		}

		private readonly IAnalytics analytics;

		private readonly string domain;
	}
}
