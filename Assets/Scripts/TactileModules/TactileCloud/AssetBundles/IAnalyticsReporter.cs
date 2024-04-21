using System;

namespace TactileModules.TactileCloud.AssetBundles
{
	public interface IAnalyticsReporter
	{
		void ReportAssetBundleDownloadCompleted(string url, int bytesDownloaded);

		void ReportAssetBundleDownloadStarted(string url);

		void ReportAssetBundleDownloadFailed(string url, int bytesDownloaded, string wwwError);

		void ReportAssetBundleLoadedFromCache(string url, int bytesDownloaded);

		void ReportAssetBundleLoadFromCacheStarted(string url);

		void ReportAssetBundleLoadFromCacheFailed(string url, int bytesDownloaded, string message);

		void ReportAssetBundleLoadFromCacheCompleted(string url, int bytesDownloaded);
	}
}
