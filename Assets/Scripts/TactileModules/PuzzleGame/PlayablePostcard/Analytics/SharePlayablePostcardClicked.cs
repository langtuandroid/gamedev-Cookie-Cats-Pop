using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Analytics
{
	[TactileAnalytics.EventAttribute("sharePlayablePostcardClicked", true)]
	public class SharePlayablePostcardClicked : BasicEvent
	{
		public SharePlayablePostcardClicked(string finalPostcard, string assetBundleName, string assetBundleUrl)
		{
			this.FinalPostcard = finalPostcard;
			this.AssetBundleName = assetBundleName;
			this.AssetBundleUrl = assetBundleUrl;
		}

		private TactileAnalytics.RequiredParam<string> FinalPostcard { get; set; }

		private TactileAnalytics.RequiredParam<string> AssetBundleName { get; set; }

		private TactileAnalytics.RequiredParam<string> AssetBundleUrl { get; set; }
	}
}
