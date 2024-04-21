using System;

namespace TactileModules.InAppPurchase
{
	public class IAPSuccessfulEventData
	{
		public IAPSuccessfulEventData(string productIdentifier, string transactionId, object platformSpecificData)
		{
			this.productIdentifier = productIdentifier;
			this.transactionId = transactionId;
			this.platformSpecificData = platformSpecificData;
		}

		public readonly string productIdentifier;

		public readonly string transactionId;

		public readonly object platformSpecificData;
	}
}
