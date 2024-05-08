using System;
using System.Collections.Generic;
using TactileModules.Analytics.Interfaces;
using TactileModules.TactileCloud.AssetBundle.AnalyticsEvents;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class AnalyticsReporter : IAnalyticsReporter
	{
		public AnalyticsReporter(IPersistableStateHandler persistableStateHandler)
		{
			this.persistableStateHandler = persistableStateHandler;
		}

		public void ReportAssetBundleDownloadCompleted(string url, int bytesDownloaded)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleDownloadCompletedEvent eventObject = new AssetBundleDownloadCompletedEvent(url, id, bytesDownloaded);
		}

		public void ReportAssetBundleDownloadStarted(string url)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleDownloadStartedEvent eventObject = new AssetBundleDownloadStartedEvent(url, id);
		}

		public void ReportAssetBundleDownloadFailed(string url, int bytesDownloaded, string wwwError)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleDownloadFailedEvent eventObject = new AssetBundleDownloadFailedEvent(url, id, bytesDownloaded, wwwError);
		}

		public void ReportAssetBundleLoadedFromCache(string url, int bytesDownloaded)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleLoadedFromCacheEvent eventObject = new AssetBundleLoadedFromCacheEvent(url, id, bytesDownloaded);
		}

		public void ReportAssetBundleLoadFromCacheStarted(string url)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleLoadFromCacheStartedEvent eventObject = new AssetBundleLoadFromCacheStartedEvent(url, id);
		}

		public void ReportAssetBundleLoadFromCacheFailed(string url, int bytesDownloaded, string message)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleLoadFromCacheFailedEvent eventObject = new AssetBundleLoadFromCacheFailedEvent(url, id, bytesDownloaded, message);
		}

		public void ReportAssetBundleLoadFromCacheCompleted(string url, int bytesDownloaded)
		{
			string id = this.UrlToExternalId(url);
			AssetBundleLoadFromCacheCompletedEvent eventObject = new AssetBundleLoadFromCacheCompletedEvent(url, id, bytesDownloaded);
		}

		private string UrlToExternalId(string url)
		{
			PersistableState persistableState = this.persistableStateHandler.Get();
			foreach (KeyValuePair<string, AssetBundleInfo> keyValuePair in persistableState.AvailableAssetBundles)
			{
				if (keyValuePair.Value.URL == url)
				{
					return keyValuePair.Value.ExternalId;
				}
			}
			return null;
		}

		private readonly IPersistableStateHandler persistableStateHandler;
	}
}
