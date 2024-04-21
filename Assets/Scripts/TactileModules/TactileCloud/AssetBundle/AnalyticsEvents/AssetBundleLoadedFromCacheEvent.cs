using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleLoadedFromCache", true)]
	public class AssetBundleLoadedFromCacheEvent : DownloadedEventBase
	{
		public AssetBundleLoadedFromCacheEvent(string url, string id, int bytesDownloaded) : base(url, id, bytesDownloaded)
		{
		}
	}
}
