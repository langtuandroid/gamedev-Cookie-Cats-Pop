using System;

namespace TactileModules.CrossPromotion.Cloud.Data
{
	public class CrossPromotionAdCreative
	{
		public CrossPromotionAdCreative()
		{
			this.AssetResolution = new CrossPromotionAdCreative.Resolution();
		}

		[JsonSerializable("id", null)]
		public string Id { get; set; }

		[JsonSerializable("url", null)]
		public string Url { get; set; }

		[JsonSerializable("name", null)]
		public string Name { get; set; }

		[JsonSerializable("resolution", null)]
		public CrossPromotionAdCreative.Resolution AssetResolution { get; set; }

		public class Resolution
		{
			[JsonSerializable("width", null)]
			public int Width { get; set; }

			[JsonSerializable("height", null)]
			public int Height { get; set; }
		}
	}
}
