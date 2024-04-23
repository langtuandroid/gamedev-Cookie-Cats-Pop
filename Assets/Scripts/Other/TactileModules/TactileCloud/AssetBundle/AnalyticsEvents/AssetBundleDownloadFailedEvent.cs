using System;
using JetBrains.Annotations;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleDownloadFailed", true)]
	public class AssetBundleDownloadFailedEvent : DownloadedEventBase
	{
		public AssetBundleDownloadFailedEvent(string url, string id, int bytesDownloaded, string downloadError) : base(url, id, bytesDownloaded)
		{
			this.DownloadError = downloadError;
		}

		private TactileAnalytics.RequiredParam<string> DownloadError { [UsedImplicitly] get; set; }
	}
}
