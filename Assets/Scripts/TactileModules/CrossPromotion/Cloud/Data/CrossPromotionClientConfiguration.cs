using System;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionClientConfiguration
	{
		[JsonSerializable("maxAdAge", null)]
		public int MaxAdAge { get; set; }

		[JsonSerializable("maxInterstitialAdsPerSession", null)]
		public int MaxInterstitialAdsPerSession { get; set; }

		[JsonSerializable("maxRewardedAdsPerSession", null)]
		public int MaxRewardedAdsPerSession { get; set; }
	}
}
