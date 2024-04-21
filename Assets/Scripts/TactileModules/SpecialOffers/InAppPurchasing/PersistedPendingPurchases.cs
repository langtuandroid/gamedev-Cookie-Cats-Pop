using System;
using TactileModules.TactilePrefs;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public class PersistedPendingPurchases : IPersistedPendingPurchases
	{
		public PersistedPendingPurchases(ILocalStorageObject<PendingPurchases> localStorageObject)
		{
			this.localStorageObject = localStorageObject;
			this.pendingPurchases = localStorageObject.Load();
		}

		public PendingPurchase GetPendingPurchase(string iapIdentifier)
		{
			if (this.pendingPurchases.Purchases.ContainsKey(iapIdentifier))
			{
				return this.pendingPurchases.Purchases[iapIdentifier];
			}
			return null;
		}

		public void SetPendingPurchase(PendingPurchase pendingPurchase)
		{
			this.pendingPurchases.Purchases[pendingPurchase.IAPIdentifier] = pendingPurchase;
			this.localStorageObject.Save(this.pendingPurchases);
		}

		public void DeletePendingPurchase(PendingPurchase pendingPurchase)
		{
			if (this.pendingPurchases.Purchases.ContainsKey(pendingPurchase.IAPIdentifier))
			{
				this.pendingPurchases.Purchases.Remove(pendingPurchase.IAPIdentifier);
				this.localStorageObject.Save(this.pendingPurchases);
			}
		}

		private readonly ILocalStorageObject<PendingPurchases> localStorageObject;

		private readonly PendingPurchases pendingPurchases;
	}
}
