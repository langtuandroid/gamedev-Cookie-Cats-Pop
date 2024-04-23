using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.InAppPurchase;

internal interface IIAP
{
	event Action<IAPSuccessfulEventData> purchaseSuccessfulEvent;

	event Action<string, string> purchaseRefundedEvent;

	event Action<List<InAppSubscription>> subscriptionUpdatedEvent;

	bool IsInitialized();

	bool InAppPurchasesEnabled();

	IEnumerator RequestProductData(List<InAppProduct> productsToRequest, Action<object> callback);

	IEnumerator RestoreTransactions(Action<object> callback);

	IEnumerator Purchase(InAppProduct product, Action<object, string, InAppPurchaseStatus> callback);

	bool IsSubscriptionActive(string productIdentifer);

	void PrintAllActiveSubscriptions();

	string GetProviderName();
}
