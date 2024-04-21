using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleDownloadStarted", true)]
	public class AssetBundleDownloadStartedEvent : EventBase
	{
		public AssetBundleDownloadStartedEvent(string url, string id) : base(url, id)
		{
		}
	}
}
