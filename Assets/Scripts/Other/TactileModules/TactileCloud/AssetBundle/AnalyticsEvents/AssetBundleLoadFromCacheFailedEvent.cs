using System;
using JetBrains.Annotations;

namespace TactileModules.TactileCloud.AssetBundle.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("assetBundleLoadFromCacheFailed", true)]
	public class AssetBundleLoadFromCacheFailedEvent : DownloadedEventBase
	{
		public AssetBundleLoadFromCacheFailedEvent(string url, string id, int bytesDownloaded, string downloadError) : base(url, id, bytesDownloaded)
		{
			this.DownloadError = downloadError;
		}

		private TactileAnalytics.RequiredParam<string> DownloadError { [UsedImplicitly] get; set; }
	}
}
