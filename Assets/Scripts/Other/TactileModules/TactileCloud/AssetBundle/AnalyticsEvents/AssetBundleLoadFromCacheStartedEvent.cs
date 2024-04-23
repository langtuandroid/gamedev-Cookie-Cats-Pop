using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleLoadFromCacheStarted", true)]
	public class AssetBundleLoadFromCacheStartedEvent : EventBase
	{
		public AssetBundleLoadFromCacheStartedEvent(string url, string id) : base(url, id)
		{
		}
	}
}
