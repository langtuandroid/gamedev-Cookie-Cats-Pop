using System;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionVideoAdCreative : CrossPromotionAdCreative
	{
		[JsonSerializable("duration", null)]
		public int Duration { get; set; }
	}
}
