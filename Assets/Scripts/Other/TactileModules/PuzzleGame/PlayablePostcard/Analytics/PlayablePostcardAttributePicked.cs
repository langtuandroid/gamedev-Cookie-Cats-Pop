using System;

namespace TactileModules.PuzzleGame.PlayablePostcard.Analytics
{
	[TactileAnalytics.EventAttribute("playablePostcardAttributePicked", true)]
	public class PlayablePostcardAttributePicked : BasicEvent
	{
		public PlayablePostcardAttributePicked(PostcardItemType itemType, string itemId, string assetBundleName, string assetBundleUrl)
		{
			this.AttributeType = itemType.ToString();
			this.AttributeId = itemId;
			this.AssetBundleName = assetBundleName;
			this.AssetBundleUrl = assetBundleUrl;
		}

		private TactileAnalytics.RequiredParam<string> AttributeType { get; set; }

		private TactileAnalytics.RequiredParam<string> AttributeId { get; set; }

		private TactileAnalytics.RequiredParam<string> AssetBundleName { get; set; }

		private TactileAnalytics.RequiredParam<string> AssetBundleUrl { get; set; }
	}
}
