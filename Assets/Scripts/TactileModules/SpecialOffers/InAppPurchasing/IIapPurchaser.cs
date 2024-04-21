using System;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public interface IIapPurchaser
	{
		event Action<PurchaseData> OnPurchaseCompleted;

		void Purchase(ISpecialOffer offer);
	}
}
