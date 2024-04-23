using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleDownloadCompleted", true)]
	public class AssetBundleDownloadCompletedEvent : DownloadedEventBase
	{
		public AssetBundleDownloadCompletedEvent(string url, string id, int bytesDownloaded) : base(url, id, bytesDownloaded)
		{
		}
	}
}
