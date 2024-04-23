using System;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public interface IPersistedPendingPurchases
	{
		PendingPurchase GetPendingPurchase(string iapIdentifier);

		void SetPendingPurchase(PendingPurchase pendingPurchase);

		void DeletePendingPurchase(PendingPurchase pendingPurchase);
	}
}
