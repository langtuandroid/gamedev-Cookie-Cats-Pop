using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleLoadFromCacheCompleted", true)]
	public class AssetBundleLoadFromCacheCompletedEvent : DownloadedEventBase
	{
		public AssetBundleLoadFromCacheCompletedEvent(string url, string id, int bytesDownloaded) : base(url, id, bytesDownloaded)
		{
		}
	}
}
