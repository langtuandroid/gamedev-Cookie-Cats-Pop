using System;
using System.Collections;

namespace TactileModules.InAppPurchase
{
	public interface IInAppPurchaseManager
	{
		event Action<InAppPurchaseManagerBase.PurchaseSuccessfulEventData> PurchaseSuccessfulEvent;

		InAppProduct GetProductForIdentifier(string identifier);

		IEnumerator DoInAppPurchase(InAppProduct product, Action<string, string, InAppPurchaseStatus> callback);
	}
}
