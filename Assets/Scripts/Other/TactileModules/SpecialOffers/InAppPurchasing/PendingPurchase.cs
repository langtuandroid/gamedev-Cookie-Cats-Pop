using System;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public class PendingPurchase
	{
		[JsonSerializable("FeatureInstanceId", null)]
		public string FeatureInstanceId { get; set; }

		[JsonSerializable("IAPIdentifier", null)]
		public string IAPIdentifier { get; set; }

		[JsonSerializable("Reward", typeof(ItemAmount))]
		public List<ItemAmount> Reward { get; set; }
	}
}
