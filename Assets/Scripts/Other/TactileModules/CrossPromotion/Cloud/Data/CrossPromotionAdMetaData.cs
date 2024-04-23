using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionAdMetaData : IPromotedGameUtilityData, IPromotedGameUtilityAdditionalData, IPromotedGameUtilityPrimaryData
	{
		[JsonSerializable("requestId", null)]
		public string RequestId { get; set; }

		[JsonSerializable("campaignId", null)]
		public string CampaignId { get; set; }

		[JsonSerializable("assetImage", null)]
		public CrossPromotionImageAssetMetaData AssetImage { get; set; }

		[JsonSerializable("assetVideo", null)]
		public CrossPromotionVideoAssetMetaData AssetVideo { get; set; }

		[JsonSerializable("buttonImageUrl", null)]
		public string ButtonImageUrl { get; set; }

		[JsonSerializable("packageName", null)]
		public string PackageName { get; set; }

		[JsonSerializable("schemeUrl", null)]
		public string SchemeUrl { get; set; }

		public string FacebookAppId { get; set; }

		[JsonSerializable("adjustToken", null)]
		public string TrackerToken { get; set; }

		[JsonSerializable("iTunesConnectId", null)]
		public string ITunesConnectId { get; set; }

		[JsonSerializable("adjustCreativeName", null)]
		public string TrackerCreativeName { get; set; }
	}
}
