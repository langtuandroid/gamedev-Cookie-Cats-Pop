using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Analytics
{
	[TactileAnalytics.EventAttribute("playablePostcardCompleted", true)]
	public class PlayablePostcardCompleted : BasicEvent
	{
		public PlayablePostcardCompleted(string finalPostcard, string assetBundleName, string assetBundleUrl)
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
