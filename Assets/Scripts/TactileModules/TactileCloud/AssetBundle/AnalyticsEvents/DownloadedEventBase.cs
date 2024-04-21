using System;
using JetBrains.Annotations;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	public class DownloadedEventBase : EventBase
	{
		public DownloadedEventBase(string url, string id, int bytesDownloaded) : base(url, id)
		{
			this.BytesDownloaded = bytesDownloaded;
		}

		private TactileAnalytics.RequiredParam<int> BytesDownloaded { [UsedImplicitly] get; set; }
	}
}
