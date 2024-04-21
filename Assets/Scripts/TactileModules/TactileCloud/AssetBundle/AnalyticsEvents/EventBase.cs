using System;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	public class EventBase
	{
		public EventBase(string url, string id)
		{
			this.AssetBundleUrl = url;
			this.AssetBundleId = id;
		}

		private TactileAnalytics.RequiredParam<string> AssetBundleUrl { get; set; }

		private TactileAnalytics.RequiredParam<string> AssetBundleId { get; set; }
	}
}
