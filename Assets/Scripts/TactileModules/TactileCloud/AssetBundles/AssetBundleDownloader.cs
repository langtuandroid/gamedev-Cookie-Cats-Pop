using System;
using System.Collections;
using Fibers;
using UnityEngine;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class AssetBundleDownloader : IAssetBundleDownloader
	{
		public AssetBundleDownloader(IAnalyticsReporter analyticsReporter)
		{
			this.analyticsReporter = analyticsReporter;
		}

		public bool IsAssetBundleCached(string url)
		{
			return Caching.IsVersionCached(url, 0);
		}

		public IEnumerator DownloadAssetBundle(string url, EnumeratorResult<DownloadResult> result)
		{
			return this.DownloadAssetBundle(url, result, null);
		}

		public IEnumerator DownloadAssetBundle(string url, EnumeratorResult<DownloadResult> result, Action<float> onProgress)
		{
			bool wasCached = this.IsAssetBundleCached(url);
			if (!wasCached)
			{
				this.analyticsReporter.ReportAssetBundleDownloadStarted(url);
			}
			WWW www = AssetBundleDownloader.LoadFromCacheOrDownload(url, 0);
			while (!www.isDone)
			{
				if (onProgress != null)
				{
					onProgress(www.progress);
				}
				yield return null;
			}
			if (onProgress != null)
			{
				onProgress(1f);
			}
			if (www.error != null)
			{
				this.analyticsReporter.ReportAssetBundleDownloadFailed(url, www.bytesDownloaded, www.error);
				string error = string.Format("Failed to download asset bundle. Url: {0}. Error: {1}", url, www.error);
				result.value = new DownloadResult(null, error);
				yield break;
			}
			if (www.assetBundle == null)
			{
				this.analyticsReporter.ReportAssetBundleDownloadFailed(url, www.bytesDownloaded, "No asset bundle in result");
				string error2 = string.Format("Failed to download asset bundle. Url: {0}. AssetBundle is null!", url);
				result.value = new DownloadResult(null, error2);
				yield break;
			}
			if (wasCached)
			{
				this.analyticsReporter.ReportAssetBundleLoadedFromCache(url, www.bytesDownloaded);
			}
			else
			{
				this.analyticsReporter.ReportAssetBundleDownloadCompleted(url, www.bytesDownloaded);
			}
			result.value = new DownloadResult(www.assetBundle, null);
			yield break;
		}

		private static WWW LoadFromCacheOrDownload(string url, int version)
		{
			return WWW.LoadFromCacheOrDownload(url, version);
		}

		public DownloadResult LoadAssetBundle(string url)
		{
			this.analyticsReporter.ReportAssetBundleLoadFromCacheStarted(url);
			if (!this.IsAssetBundleCached(url))
			{
				string text = string.Format("AssetBundle '{0}' failed to load, because it was not cached.", url);
				this.analyticsReporter.ReportAssetBundleLoadFromCacheFailed(url, 0, text);
				return new DownloadResult(null, text);
			}
			EnumeratorResult<DownloadResult> enumeratorResult = new EnumeratorResult<DownloadResult>();
			EnumeratorResult<int> enumeratorResult2 = new EnumeratorResult<int>();
			IEnumerator enumerator = this.AsyncLoadAssetBundle(url, enumeratorResult, enumeratorResult2);
			while (enumerator.MoveNext())
			{
			}
			if (enumeratorResult.value.Success)
			{
				this.analyticsReporter.ReportAssetBundleLoadFromCacheCompleted(url, enumeratorResult2.value);
			}
			else
			{
				this.analyticsReporter.ReportAssetBundleLoadFromCacheFailed(url, enumeratorResult2.value, enumeratorResult.value.error);
			}
			return enumeratorResult.value;
		}

		private IEnumerator AsyncLoadAssetBundle(string url, EnumeratorResult<DownloadResult> result, EnumeratorResult<int> bytesDownloadedResult)
		{
			while (!Caching.ready)
			{
				yield return null;
			}
			using (WWW www = WWW.LoadFromCacheOrDownload(url, 0))
			{
				yield return www;
				if (www.error != null)
				{
					string error = string.Format("Failed to download asset bundle. Url: {0}. Error: {1}", url, www.error);
					result.value = new DownloadResult(null, error);
				}
				else
				{
					result.value = new DownloadResult(www.assetBundle, string.Empty);
				}
				bytesDownloadedResult.value = www.bytesDownloaded;
			}
			yield break;
		}

		private readonly IAnalyticsReporter analyticsReporter;
	}
}
