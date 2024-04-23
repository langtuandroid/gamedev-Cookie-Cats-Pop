using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Prime31;
using TactileModules.InAppPurchase;
using UnityEngine;

public class IAPPlayStore : IIAP
{
	public IAPPlayStore(string publicKey)
	{
		GoogleIABManager.purchaseSucceededEvent += this.PurchaseSucceededHandler;
		GoogleIABManager.consumePurchaseSucceededEvent += this.PurchaseConsumedHandler;
		GoogleIABManager.billingSupportedEvent += this.BillingSupportedHandler;
		GoogleIABManager.billingNotSupportedEvent += this.BillingNotSupportedHandler;
		GoogleIAB.init(publicKey);
	}

	~IAPPlayStore()
	{
		GoogleIABManager.purchaseSucceededEvent -= this.PurchaseSucceededHandler;
		GoogleIABManager.consumePurchaseSucceededEvent -= this.PurchaseConsumedHandler;
		GoogleIABManager.billingSupportedEvent -= this.BillingSupportedHandler;
		GoogleIABManager.billingNotSupportedEvent -= this.BillingNotSupportedHandler;
	}

	public bool IsInitialized()
	{
		return this.isInitialized;
	}

	public bool InAppPurchasesEnabled()
	{
		return this.billingSupported;
	}

	public IEnumerator RequestProductData(List<InAppProduct> productsToRequest, Action<object> callback)
	{
		if (!this.IsInitialized())
		{
			yield break;
		}
		if (!this.InAppPurchasesEnabled())
		{
			yield break;
		}
		while (this.purchaseInProgress)
		{
			yield return null;
		}
		bool callbackReceived = false;
		List<GoogleSkuInfo> products = new List<GoogleSkuInfo>();
		List<GooglePurchase> unconsumedPurchases = new List<GooglePurchase>();
		string error = null;
		Action<List<GooglePurchase>, List<GoogleSkuInfo>> queryInventorySucceededHandler = delegate(List<GooglePurchase> receivedPurchases, List<GoogleSkuInfo> receivedProducts)
		{
			this.subscriptionDurationPeriods.Clear();
			foreach (GoogleSkuInfo googleSkuInfo2 in receivedProducts)
			{
				this.subscriptionDurationPeriods.Add(googleSkuInfo2.productId, googleSkuInfo2.subscriptionPeriod);
			}
			foreach (GooglePurchase googlePurchase in receivedPurchases)
			{
			}
			products = receivedProducts;
			unconsumedPurchases = receivedPurchases;
			callbackReceived = true;
		};
		Action<string> queryInventoryFailedHandler = delegate(string receivedError)
		{
			callbackReceived = true;
			error = receivedError;
		};
		GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededHandler;
		GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedHandler;
		List<string> identifiers = new List<string>();
		foreach (InAppProduct inAppProduct in productsToRequest)
		{
			identifiers.Add(inAppProduct.Identifier);
		}
		GoogleIAB.queryInventory(identifiers.ToArray());
		while (!callbackReceived)
		{
			yield return null;
		}
		GoogleIABManager.queryInventorySucceededEvent -= queryInventorySucceededHandler;
		GoogleIABManager.queryInventoryFailedEvent -= queryInventoryFailedHandler;
		if (error == null)
		{
			foreach (GoogleSkuInfo googleSkuInfo in products)
			{
				foreach (InAppProduct inAppProduct2 in productsToRequest)
				{
					if (inAppProduct2.Identifier == googleSkuInfo.productId)
					{
						inAppProduct2.StoreInfo = new InAppProductStoreInfo
						{
							identifier = googleSkuInfo.productId,
							title = googleSkuInfo.title,
							description = googleSkuInfo.description,
							price = googleSkuInfo.price,
							formattedPrice = googleSkuInfo.formattedPrice,
							currencyCode = googleSkuInfo.priceCurrencyCode,
							currencySymbol = string.Empty
						};
					}
				}
			}
			this.cachedProducts = productsToRequest;
			this.purchasedActiveSubscriptions.Clear();
			foreach (GooglePurchase ucp in unconsumedPurchases)
			{
				InAppProduct p = this.GetProductForProductId(ucp.productId);
				if (p != null)
				{
					if (p.Consumable)
					{
						if (ucp.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
						{
							IEnumerator e = this.ConsumePurchase(ucp, delegate(object err, GooglePurchase consumedPurchase)
							{
							});
							do
							{
								yield return e.Current;
							}
							while (e.MoveNext());
						}
					}
					else
					{
						this.EmitPurchaseSuccessfulEvent(ucp);
						if (ucp.type == "subs")
						{
							this.purchasedActiveSubscriptions.Add(ucp);
						}
					}
				}
			}
			this.UpdateSubscriptionsList(this.purchasedActiveSubscriptions);
		}
		callback(error);
		yield break;
	}

	public IEnumerator RestoreTransactions(Action<object> callback)
	{
		if (!this.IsInitialized())
		{
			yield break;
		}
		if (!this.InAppPurchasesEnabled())
		{
			yield break;
		}
		yield return null;
		callback(null);
		yield break;
	}

	public IEnumerator Purchase(InAppProduct product, Action<object, string, InAppPurchaseStatus> callback)
	{
		if (!this.IsInitialized())
		{
			yield break;
		}
		if (!this.InAppPurchasesEnabled())
		{
			yield break;
		}
		while (this.purchaseInProgress)
		{
			yield return null;
		}
		this.purchaseInProgress = true;
		bool callbackReceived = false;
		GooglePurchase purchase = null;
		InAppPurchaseStatus status = InAppPurchaseStatus.Failed;
		object error = null;
		string uuid = Guid.NewGuid().ToString();
		Action<string, int> purchaseFailedHandler = delegate(string receivedError, int errorCode)
		{
			callbackReceived = true;
			if (receivedError.IndexOf("response: 1:User Canceled") != -1 || receivedError.IndexOf("response: -1005:User cancelled") != -1)
			{
				status = InAppPurchaseStatus.Cancelled;
			}
			else
			{
				status = InAppPurchaseStatus.Failed;
			}
			error = receivedError;
		};
		Action<GooglePurchase> purchaseSucceededHandler = delegate(GooglePurchase receivedPurchase)
		{
			if (receivedPurchase.developerPayload == uuid)
			{
				callbackReceived = true;
				if (receivedPurchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
				{
					status = InAppPurchaseStatus.Succeeded;
					purchase = receivedPurchase;
				}
				else
				{
					status = InAppPurchaseStatus.Cancelled;
				}
			}
		};
		Action handleOnResume = delegate()
		{
			callbackReceived = true;
		};
		ActivityManager.onResumeNeverIgnoredEvent += handleOnResume;
		GoogleIABManager.purchaseFailedEvent += purchaseFailedHandler;
		GoogleIABManager.purchaseSucceededEvent += purchaseSucceededHandler;
		ActivityManager.PushIgnoreNextOnResumeEventIfNoNewIntent();
		GoogleIAB.purchaseProduct(product.Identifier, uuid);
		while (!callbackReceived)
		{
			yield return null;
		}
		ActivityManager.PopIgnoreNextOnResumeEventIfNoNewIntent();
		GoogleIABManager.purchaseFailedEvent -= purchaseFailedHandler;
		GoogleIABManager.purchaseSucceededEvent -= purchaseSucceededHandler;
		ActivityManager.onResumeNeverIgnoredEvent -= handleOnResume;
		if (status == InAppPurchaseStatus.Succeeded && product.Consumable)
		{
			IEnumerator e = this.ConsumePurchase(purchase, delegate(object err, GooglePurchase consumedPurchase)
			{
				error = err;
				purchase = consumedPurchase;
			});
			do
			{
				yield return e.Current;
			}
			while (e.MoveNext());
		}
		callback(error, (purchase == null) ? null : purchase.purchaseToken, status);
		this.purchaseInProgress = false;
		yield break;
	}

	private IEnumerator ConsumePurchase(GooglePurchase purchase, Action<object, GooglePurchase> callback)
	{
		bool callbackReceived = false;
		string error = null;
		GooglePurchase consumedPurchase = null;
		Action<string> consumePurchaseFailedHandler = delegate(string receivedError)
		{
			IAPAnalytics.LogPurchaseConsumeFailed(purchase);
			callbackReceived = true;
			error = receivedError;
		};
		Action<GooglePurchase> consumePurchaseSucceededHandler = delegate(GooglePurchase receivedPurchase)
		{
			if (receivedPurchase.developerPayload == purchase.developerPayload)
			{
				callbackReceived = true;
				consumedPurchase = receivedPurchase;
			}
		};
		GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedHandler;
		GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededHandler;
		int tries = 3;
		while (tries > 0 && consumedPurchase == null)
		{
			callbackReceived = false;
			GoogleIAB.consumeProduct(purchase.productId);
			while (!callbackReceived)
			{
				yield return null;
			}
			tries--;
		}
		GoogleIABManager.consumePurchaseFailedEvent -= consumePurchaseFailedHandler;
		GoogleIABManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededHandler;
		callback(error, consumedPurchase);
		yield break;
	}

	private void BillingSupportedHandler()
	{
		this.billingSupported = true;
		this.isInitialized = true;
	}

	private void BillingNotSupportedHandler(string error)
	{
		this.billingSupported = false;
		this.isInitialized = true;
	}

	private InAppProduct GetProductForProductId(string productID)
	{
		foreach (InAppProduct inAppProduct in this.cachedProducts)
		{
			if (inAppProduct.Identifier == productID)
			{
				return inAppProduct;
			}
		}
		return null;
	}

	private void EmitPurchaseSuccessfulEvent(GooglePurchase purchase)
	{
		if (this.purchaseSuccessfulEvent != null)
		{
			this.purchaseSuccessfulEvent(new IAPSuccessfulEventData(purchase.productId, purchase.purchaseToken, purchase));
		}
	}

	private void EmitPurchaseRefundedEvent(GooglePurchase purchase)
	{
		if (this.purchaseRefundedEvent != null)
		{
			this.purchaseRefundedEvent(purchase.productId, purchase.purchaseToken);
		}
	}

	private void PurchaseSucceededHandler(GooglePurchase purchase)
	{
		InAppProduct productForProductId = this.GetProductForProductId(purchase.productId);
		if (productForProductId != null && !productForProductId.Consumable)
		{
			if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
			{
				this.EmitPurchaseSuccessfulEvent(purchase);
				if (purchase.type == "subs")
				{
					this.UpdateSubscriptionsList(new List<GooglePurchase>
					{
						purchase
					});
				}
			}
			else if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Refunded)
			{
				this.EmitPurchaseRefundedEvent(purchase);
			}
		}
	}

	private void PurchaseConsumedHandler(GooglePurchase purchase)
	{
		InAppProduct productForProductId = this.GetProductForProductId(purchase.productId);
		if (productForProductId != null && purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
		{
			this.EmitPurchaseSuccessfulEvent(purchase);
		}
	}

	public bool IsSubscriptionActive(string productIdentifier)
	{
		List<InAppSubscription> localPersistedSubscriptionData = this.LocalPersistedSubscriptionData;
		for (int i = 0; i < localPersistedSubscriptionData.Count; i++)
		{
			InAppSubscription inAppSubscription = localPersistedSubscriptionData[i];
			if (inAppSubscription.ProductId == productIdentifier)
			{
				return true;
			}
		}
		return false;
	}

	public void PrintAllActiveSubscriptions()
	{
		List<InAppSubscription> localPersistedSubscriptionData = this.LocalPersistedSubscriptionData;
		for (int i = 0; i < localPersistedSubscriptionData.Count; i++)
		{
		}
	}

	public string GetProviderName()
	{
		return "google";
	}

	private void UpdateSubscriptionsList(List<GooglePurchase> storeActiveSubscriptions)
	{
		if (storeActiveSubscriptions == null)
		{
			return;
		}
		List<InAppSubscription> localPersistedSubscriptionData = this.LocalPersistedSubscriptionData;
		for (int i = localPersistedSubscriptionData.Count - 1; i >= 0; i--)
		{
			InAppSubscription inAppSubscription = localPersistedSubscriptionData[i];
			bool flag = false;
			for (int j = 0; j < storeActiveSubscriptions.Count; j++)
			{
				if (inAppSubscription.ProductId == storeActiveSubscriptions[i].productId)
				{
					flag = true;
					long intDate = storeActiveSubscriptions[i].purchaseTime / 1000L;
					inAppSubscription.PurchaseDate = intDate.ToDateTimeFromEpoch();
					break;
				}
			}
			if (!flag)
			{
				localPersistedSubscriptionData.RemoveAt(i);
			}
		}
		for (int k = 0; k < storeActiveSubscriptions.Count; k++)
		{
			GooglePurchase googlePurchase = storeActiveSubscriptions[k];
			bool flag2 = false;
			for (int l = 0; l < localPersistedSubscriptionData.Count; l++)
			{
				if (googlePurchase.productId == localPersistedSubscriptionData[l].ProductId)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				InAppSubscription inAppSubscription2 = new InAppSubscription();
				inAppSubscription2.Platform = RuntimePlatform.Android;
				inAppSubscription2.ProductId = googlePurchase.productId;
				long intDate2 = googlePurchase.purchaseTime / 1000L;
				inAppSubscription2.PurchaseDate = intDate2.ToDateTimeFromEpoch();
				localPersistedSubscriptionData.Add(inAppSubscription2);
			}
		}
		this.LocalPersistedSubscriptionData = localPersistedSubscriptionData;
		if (this.subscriptionUpdatedEvent != null)
		{
			this.subscriptionUpdatedEvent(this.LocalPersistedSubscriptionData);
		}
	}

	private List<InAppSubscription> LocalPersistedSubscriptionData
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("subscriptionDataGoogle", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.ArrayListToGenericList<InAppSubscription>(securedString.arrayListFromJson());
			}
			return new List<InAppSubscription>();
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("subscriptionDataGoogle", JsonSerializer.GenericListToArrayList<InAppSubscription>(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("subscriptionDataGoogle", string.Empty);
			}
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<IAPSuccessfulEventData> purchaseSuccessfulEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string, string> purchaseRefundedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<List<InAppSubscription>> subscriptionUpdatedEvent;

	private Dictionary<string, string> subscriptionDurationPeriods = new Dictionary<string, string>();

	private bool isInitialized;

	private bool billingSupported;

	private bool purchaseInProgress;

	private List<InAppProduct> cachedProducts = new List<InAppProduct>();

	private List<GooglePurchase> purchasedActiveSubscriptions = new List<GooglePurchase>();

	private const string subscriptinPersistableKey = "subscriptionDataGoogle";
}
