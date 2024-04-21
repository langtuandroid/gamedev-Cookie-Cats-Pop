using System;
using System.Collections.Generic;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public class PendingPurchases
	{
		public PendingPurchases()
		{
			this.Purchases = new Dictionary<string, PendingPurchase>();
		}

		[JsonSerializable("PendingPurchases", typeof(PendingPurchase))]
		public Dictionary<string, PendingPurchase> Purchases { get; set; }
	}
}
