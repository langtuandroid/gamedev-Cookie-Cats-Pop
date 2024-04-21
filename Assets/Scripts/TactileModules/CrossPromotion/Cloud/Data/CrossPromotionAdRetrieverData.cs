using System;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionAdRetrieverData
	{
		[JsonSerializable("AdMetaData", null)]
		public CrossPromotionAdMetaData AdMetaData { get; set; }

		[JsonSerializable("RequestTimestamp", null)]
		public DateTime RequestTimestamp { get; set; }
	}
}
