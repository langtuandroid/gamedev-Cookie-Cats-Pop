using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Prime31;
using TactileModules.InAppPurchase;
using UnityEngine;

public abstract class InAppPurchaseManagerBase : IInAppPurchaseManager
{
	public InAppPurchaseManagerBase(FacebookClient fbClient, List<InAppProductTactileInfo> tactileInAppProducts, string androidPublicKey)
	{
		this.fbClient = fbClient;
		this.inAppProducts = new List<InAppProduct>();
		foreach (InAppProductTactileInfo tactileInfo in tactileInAppProducts)
		{
			this.inAppProducts.Add(new InAppProduct(tactileInfo));
		}
		this.iap = new IAPPlayStore(androidPublicKey);
		this.iap.purchaseSuccessfulEvent += this.PurchaseSuccessfulHandler;
		this.iap.subscriptionUpdatedEvent += this.SubscriptionUpdatedHandler;
		this.iap.purchaseRefundedEvent += delegate(string productIdentifier, string transactionId)
		{
			this.PurchaseRefundedHandler(productIdentifier, transactionId);
			if (this.PurchaseRefundedEvent != null)
			{
				this.PurchaseRefundedEvent(new InAppPurchaseManagerBase.PurchaseRefundedEventData
				{
					ProductId = productIdentifier,
					TransactionId = transactionId
				});
			}
		};
		FiberCtrl.Pool.Run(this.RequestProductDataWhenIAPInitialized(), false);
		ActivityManager.onResumeEvent += this.ApplicationWillEnterForeground;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnProductDataUpdated = delegate ()
    {
    };



    protected abstract void PurchaseRefundedHandler(string productIdentifier, string transactionId);

	public abstract IEnumerator DoInAppPurchase(InAppProduct product);

	public abstract IEnumerator DoInAppPurchase(InAppProduct product, Action<string, string, InAppPurchaseStatus> callback);

	protected void UpdateInAppProducts(List<InAppProductTactileInfo> tactileInAppProducts)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (InAppProductTactileInfo inAppProductTactileInfo in tactileInAppProducts)
		{
			InAppProduct productForIdentifier = this.GetProductForIdentifier(inAppProductTactileInfo.FullIdentifier);
			if (productForIdentifier != null)
			{
				productForIdentifier.TactileInfo = inAppProductTactileInfo;
			}
			else
			{
				this.inAppProducts.Add(new InAppProduct(inAppProductTactileInfo));
			}
			hashSet.Add(inAppProductTactileInfo.FullIdentifier);
		}
		for (int i = this.inAppProducts.Count - 1; i >= 0; i--)
		{
			if (!hashSet.Contains(this.inAppProducts[i].Identifier))
			{
				this.inAppProducts.RemoveAt(i);
			}
		}
		FiberCtrl.Pool.Run(this.RequestProductDataWhenIAPInitialized(), false);
	}

	private void ApplicationWillEnterForeground()
	{
		FiberCtrl.Pool.Run(this.RequestProductDataWhenIAPInitialized(), false);
	}

	private IEnumerator EnsureProductDataRequestedSuccessfully(float timeout)
	{
		if (!this.productDataRequestInProgres && !this.productDataRequestedSuccessfully)
		{
			FiberCtrl.Pool.Run(this.RequestProductDataWhenIAPInitialized(), false);
		}
		float startTime = Time.realtimeSinceStartup;
		while (this.productDataRequestInProgres)
		{
			if (Time.realtimeSinceStartup - startTime > timeout)
			{
				yield break;
			}
			yield return null;
		}
		yield break;
	}

	private IEnumerator RequestProductDataWhenIAPInitialized()
	{
		while (this.productDataRequestInProgres)
		{
			yield return null;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.productDataRequestInProgres = false;
		});
		this.productDataRequestInProgres = true;
		while (!this.iap.IsInitialized())
		{
			yield return null;
		}
		if (!this.iap.InAppPurchasesEnabled())
		{
			yield break;
		}
		yield return this.iap.RequestProductData(this.inAppProducts, delegate(object error)
		{
			if (error == null)
			{
				this.productDataRequestedSuccessfully = true;
			}
			else
			{
				this.productDataRequestedSuccessfully = false;
			}
			this.OnProductDataUpdated();
		});
		yield break;
	}

	protected IEnumerator RestoreTransactions(Action<object> callback)
	{
		float startTime = Time.realtimeSinceStartup;
		float maxTime = 15f;
		while (!this.iap.IsInitialized())
		{
			if (Time.realtimeSinceStartup - startTime > maxTime)
			{
				callback("Timed out. Unable to contact the store.");
				yield break;
			}
			yield return null;
		}
		yield return this.EnsureProductDataRequestedSuccessfully(maxTime);
		if (!this.productDataRequestedSuccessfully)
		{
			callback("Timed out. Unable to contact the store.");
			yield break;
		}
		if (!this.iap.InAppPurchasesEnabled())
		{
			callback("In app purchases are not enabled. Not requesting product data from store.");
			yield break;
		}
		object error = null;
		yield return this.iap.RestoreTransactions(delegate(object receivedError)
		{
			error = receivedError;
		});
		callback(error);
		yield break;
	}

	protected IEnumerator Purchase(InAppProduct product, Action<object, string, string, InAppPurchaseStatus> callback)
	{
		if (this.activePurchaseSession != null)
		{
			string text = "In App Purchase already in progress.";
			string text2 = Guid.NewGuid().ToString();
			IAPAnalytics.LogPurchaseStarted(text2, product);
			IAPAnalytics.LogPurchaseFailed(text2, product, text);
			callback(text, text2, null, InAppPurchaseStatus.Failed);
			yield break;
		}
		this.activePurchaseSession = new InAppPurchaseManagerBase.PurchaseSession
		{
			SessionId = Guid.NewGuid().ToString(),
			Product = product
		};
		yield return new Fiber.OnExit(delegate()
		{
			this.activePurchaseSession = null;
		});
		IAPAnalytics.LogPurchaseStarted(this.activePurchaseSession.SessionId, product);
		float startTime = Time.realtimeSinceStartup;
		float maxTime = 15f;
		while (!this.iap.IsInitialized())
		{
			if (Time.realtimeSinceStartup - startTime > maxTime)
			{
				string text3 = "Timed out. In App Purchase system failed to initialize.";
				IAPAnalytics.LogPurchaseFailed(this.activePurchaseSession.SessionId, product, text3);
				callback(text3, this.activePurchaseSession.SessionId, null, InAppPurchaseStatus.Failed);
				yield break;
			}
			yield return null;
		}
		if (!this.iap.InAppPurchasesEnabled())
		{
			string text4 = "In App Purchases are not enabled on device. Not able to continue.";
			IAPAnalytics.LogPurchaseFailed(this.activePurchaseSession.SessionId, product, text4);
			callback(text4, this.activePurchaseSession.SessionId, null, InAppPurchaseStatus.Failed);
			yield break;
		}
		yield return this.EnsureProductDataRequestedSuccessfully(maxTime);
		if (!this.productDataRequestedSuccessfully)
		{
			string text5 = "Timed out. Faild to request product data from In App Purchase provider.";
			IAPAnalytics.LogPurchaseFailed(this.activePurchaseSession.SessionId, product, text5);
			callback(text5, this.activePurchaseSession.SessionId, null, InAppPurchaseStatus.Failed);
			yield break;
		}
		object error = null;
		InAppPurchaseStatus status = InAppPurchaseStatus.Failed;
		string transactionId = null;
		yield return this.iap.Purchase(product, delegate(object receivedError, string receivedTransactionId, InAppPurchaseStatus receivedStatus)
		{
			error = receivedError;
			transactionId = receivedTransactionId;
			status = receivedStatus;
		});
		if (status == InAppPurchaseStatus.Failed)
		{
			IAPAnalytics.LogPurchaseFailed(this.activePurchaseSession.SessionId, product, (error == null) ? string.Empty : error.ToString());
		}
		else if (status == InAppPurchaseStatus.Cancelled)
		{
			IAPAnalytics.LogPurchaseCancelled(this.activePurchaseSession.SessionId, product, (error == null) ? string.Empty : error.ToString());
		}
		else if (status == InAppPurchaseStatus.Initiated)
		{
			IAPAnalytics.LogPurchaseInitiated(this.activePurchaseSession.SessionId, product);
		}
		else if (status == InAppPurchaseStatus.Succeeded)
		{
			IAPAnalytics.LogPurchaseSucceeded(this.activePurchaseSession.SessionId, product, transactionId);
		}
		callback(error, this.activePurchaseSession.SessionId, transactionId, status);
		yield break;
	}

	protected virtual void PurchaseSuccessfulHandler(IAPSuccessfulEventData iapData)
	{
		string text = "NO_ACTIVE_PURCHASE_SESSION";
		InAppProduct inAppProduct = this.GetProductForIdentifier(iapData.productIdentifier);
		if (this.activePurchaseSession != null && iapData.productIdentifier.Equals(this.activePurchaseSession.Product.Identifier))
		{
			text = this.activePurchaseSession.SessionId;
			inAppProduct = this.activePurchaseSession.Product;
		}
		if (inAppProduct == null)
		{
			IAPAnalytics.LogPurchaseSuccessfulUnknownIdentifier(iapData.productIdentifier, text, iapData.transactionId, (iapData.platformSpecificData == null) ? string.Empty : iapData.platformSpecificData.ToString());
			return;
		}
		this.MarkPresumablyPayingUser();
		string value = string.Empty;
		double num = 0.0;
		if (inAppProduct.StoreInfo != null)
		{
			value = inAppProduct.StoreInfo.currencyCode;
			double.TryParse(inAppProduct.StoreInfo.price, out num);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"productId",
				inAppProduct.TactileInfo.FullIdentifier
			},
			{
				"externalId",
				(this.fbClient.CachedMe == null) ? string.Empty : this.fbClient.CachedMe.Id
			},
			{
				"price",
				inAppProduct.TactileInfo.Price.ToString()
			},
			{
				"currency",
				inAppProduct.TactileInfo.CurrencyCode
			},
			{
				"localPrice",
				num.ToString()
			},
			{
				"localCurrencyCode",
				value
			},
			{
				"title",
				inAppProduct.TactileInfo.Title
			},
			{
				"description",
				inAppProduct.TactileInfo.Description
			},
			{
				"fbAppEventStatus",
				"reported-from-device"
			},
			{
				"purchaseType",
				(!(inAppProduct.ProductType == "Auto-Renewable Subscription")) ? "purchase" : "subscription"
			},
			{
				"purchaseSessionId",
				text
			}
		};
		string transactionReceipt = string.Empty;
		GooglePurchase googlePurchase = (GooglePurchase)iapData.platformSpecificData;
		string text2;
		if (googlePurchase.orderId != null)
		{
			text2 = googlePurchase.orderId;
		}
		else
		{
			text2 = "SandboxOrder." + Guid.NewGuid().ToString();
		}
		dictionary["store"] = "googleplay";
		dictionary["orderId"] = text2;
		dictionary["signature"] = googlePurchase.signature;
		transactionReceipt = googlePurchase.originalJson;
		string signature = googlePurchase.signature;
		DateTime utcNow = DateTime.UtcNow;
		IAPAnalytics.TransactionEventData transactionEventData = new IAPAnalytics.TransactionEventData(inAppProduct, 1, text, iapData.transactionId, transactionReceipt, text2, utcNow);
		IAPAnalytics.LogUnvalidatedPurchase(transactionEventData, signature, this.iap.GetProviderName());
		if (this.PurchaseSuccessfulEvent != null)
		{
			this.PurchaseSuccessfulEvent(new InAppPurchaseManagerBase.PurchaseSuccessfulEventData
			{
				ProductId = iapData.productIdentifier,
				PurchaseSessionId = text,
				TransactionId = iapData.transactionId
			});
		}
		this.SetPayingUser();
	}

	protected abstract void SetPayingUser();

	public InAppProduct GetProductForIdentifier(string identifier)
	{
		foreach (InAppProduct inAppProduct in this.inAppProducts)
		{
			if (inAppProduct.Identifier == identifier)
			{
				return inAppProduct;
			}
		}
		return null;
	}

	public bool UserIsCheating
	{
		get
		{
			return TactilePlayerPrefs.GetBool("UserIsCheating", false);
		}
		private set
		{
			TactilePlayerPrefs.SetBool("UserIsCheating", value);
		}
	}

	public bool IsPresumablyPayingUser
	{
		get
		{
			return !this.UserIsCheating && TactilePlayerPrefs.GetBool("PresumablyPayingUser", false);
		}
	}

	public void MarkPresumablyPayingUser()
	{
		TactilePlayerPrefs.SetBool("PresumablyPayingUser", true);
	}

	public bool IsSubscriptionActive(string productIdentifier)
	{
		return this.iap.IsSubscriptionActive(productIdentifier);
	}

	public void PrintAllActiveSubscriptions()
	{
		this.iap.PrintAllActiveSubscriptions();
	}

	private void SubscriptionUpdatedHandler(List<InAppSubscription> inAppSubscriptions)
	{
		if (this.SubscriptionsListUpdatedEvent != null)
		{
			this.SubscriptionsListUpdatedEvent(inAppSubscriptions);
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InAppPurchaseManagerBase.PurchaseSuccessfulEventData> PurchaseSuccessfulEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<InAppPurchaseManagerBase.PurchaseRefundedEventData> PurchaseRefundedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<List<InAppSubscription>> SubscriptionsListUpdatedEvent;

	private readonly FacebookClient fbClient;

	private readonly IIAP iap;

	private readonly List<InAppProduct> inAppProducts;

	private InAppPurchaseManagerBase.PurchaseSession activePurchaseSession;

	private bool productDataRequestedSuccessfully;

	private bool productDataRequestInProgres;

	private bool processTransactionDataToVerifyInProgress;

	public class PurchaseSession
	{
		public string SessionId { get; set; }

		public InAppProduct Product { get; set; }
	}

	public class PurchaseSuccessfulEventData
	{
		public string ProductId { get; set; }

		public string PurchaseSessionId { get; set; }

		public string TransactionId { get; set; }
	}

	public class PurchaseRefundedEventData
	{
		public string ProductId { get; set; }

		public string TransactionId { get; set; }
	}
}
