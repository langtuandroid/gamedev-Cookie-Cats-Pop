using System;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionVideoAssetMetaData : ICrossPromotionAdAssetMetaData
	{
		[JsonSerializable("portrait", null)]
		public CrossPromotionVideoAdCreative Portrait { get; set; }

		[JsonSerializable("landscape", null)]
		public CrossPromotionVideoAdCreative Landscape { get; set; }

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
