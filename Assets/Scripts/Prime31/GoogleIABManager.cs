using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Prime31
{
	public class GoogleIABManager : AbstractManager
	{
		static GoogleIABManager()
		{
			AbstractManager.initialize(typeof(GoogleIABManager));
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action billingSupportedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string> billingNotSupportedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<List<GooglePurchase>, List<GoogleSkuInfo>> queryInventorySucceededEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string> queryInventoryFailedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string, string> purchaseCompleteAwaitingVerificationEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GooglePurchase> purchaseSucceededEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string, int> purchaseFailedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<GooglePurchase> consumePurchaseSucceededEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<string> consumePurchaseFailedEvent;

		public void billingSupported(string empty)
		{
			GoogleIABManager.billingSupportedEvent.fire();
		}

		public void billingNotSupported(string error)
		{
			GoogleIABManager.billingNotSupportedEvent.fire(error);
		}

		public void queryInventorySucceeded(string json)
		{
			if (GoogleIABManager.queryInventorySucceededEvent != null)
			{
				Dictionary<string, object> dictionary = json.dictionaryFromJson();
				GoogleIABManager.queryInventorySucceededEvent(GooglePurchase.fromList(dictionary["purchases"] as List<object>), GoogleSkuInfo.fromList(dictionary["skus"] as List<object>));
			}
		}

		public void queryInventoryFailed(string error)
		{
			GoogleIABManager.queryInventoryFailedEvent.fire(error);
		}

		public void purchaseCompleteAwaitingVerification(string json)
		{
			if (GoogleIABManager.purchaseCompleteAwaitingVerificationEvent != null)
			{
				Dictionary<string, object> dictionary = json.dictionaryFromJson();
				string arg = dictionary["purchaseData"].ToString();
				string arg2 = dictionary["signature"].ToString();
				GoogleIABManager.purchaseCompleteAwaitingVerificationEvent(arg, arg2);
			}
		}

		public void purchaseSucceeded(string json)
		{
			GoogleIABManager.purchaseSucceededEvent.fire(new GooglePurchase(json.dictionaryFromJson()));
		}

		public void purchaseFailed(string json)
		{
			if (GoogleIABManager.purchaseFailedEvent != null)
			{
				Dictionary<string, object> dictionary = Json.decode<Dictionary<string, object>>(json, null);
				GoogleIABManager.purchaseFailedEvent(dictionary["result"].ToString(), int.Parse(dictionary["response"].ToString()));
			}
		}

		public void consumePurchaseSucceeded(string json)
		{
			if (GoogleIABManager.consumePurchaseSucceededEvent != null)
			{
				GoogleIABManager.consumePurchaseSucceededEvent.fire(new GooglePurchase(json.dictionaryFromJson()));
			}
		}

		public void consumePurchaseFailed(string error)
		{
			GoogleIABManager.consumePurchaseFailedEvent.fire(error);
		}
	}
}
