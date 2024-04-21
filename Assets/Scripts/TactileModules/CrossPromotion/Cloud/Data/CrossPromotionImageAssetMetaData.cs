using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionImageAssetMetaData : ICrossPromotionAdAssetMetaData
	{
		[JsonSerializable("portrait", null)]
		public CrossPromotionImageAdCreative Portrait { get; set; }

		[JsonSerializable("landscape", null)]
		public CrossPromotionImageAdCreative Landscape { get; set; }

		public CrossPromotionAdCreative GetPortrait()
		{
			return this.Portrait;
		}

		public CrossPromotionAdCreative GetLandscape()
		{
			return this.Landscape;
		}
	}
}
