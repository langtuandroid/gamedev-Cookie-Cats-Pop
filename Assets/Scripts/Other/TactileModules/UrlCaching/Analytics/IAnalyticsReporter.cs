using System;

namespace TactileModules.UrlCaching.Analytics
{
	public interface IAnalyticsReporter
	{
		void ReportUrlCachingStarted(string url);

		void ReportUrlCachingCompleted(string url, int cachedFileSize);

		void ReportUrlCachingWriteAllBytesStarted(string url);

		void ReportUrlCachingWriteAllBytesCompleted(string url);

		void ReportUrlCachingError(string url, string errorName, string errorMessage, Exception exception);
	}
}
