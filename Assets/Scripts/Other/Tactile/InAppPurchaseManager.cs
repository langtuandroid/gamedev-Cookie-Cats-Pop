using System;
using System.Collections;

namespace Tactile
{
	public class InAppPurchaseManager : InAppPurchaseManagerBase
	{
		public InAppPurchaseManager(IDialogViewProvider dialogProvider, FacebookClient fbClient, CloudClientBase cloudClient, ConfigurationManager configManager) : base(fbClient, configManager.GetConfig<IAPConfig>().InAppProducts, Constants.ANDROID_PUBLIC_KEY)
		{
			this.dialogProvider = dialogProvider;
			configManager.ConfigurationUpdated += this.ConfigurationUpdatedHandler;
		}

		protected override void PurchaseRefundedHandler(string productIdentifier, string transactionId)
		{
			if (base.GetProductForIdentifier(productIdentifier) == null)
			{
				return;
			}
		}

		protected override void SetPayingUser()
		{
			UserSettingsManager.Get<InAppPurchaseManager.PersistableState>().IsPayingUser = true;
			UserSettingsManager.Instance.SaveLocalSettings();
		}

		private void ConfigurationUpdatedHandler()
		{
			base.UpdateInAppProducts(ConfigurationManager.Get<IAPConfig>().InAppProducts);
		}

		public override IEnumerator DoInAppPurchase(InAppProduct product)
		{
			return this.DoInAppPurchase(product, null);
		}

		public override IEnumerator DoInAppPurchase(InAppProduct product, Action<string, string, InAppPurchaseStatus> callback)
		{
			DateTime time0 = DateTime.UtcNow;
			object progressView = this.dialogProvider.ShowProgressView(L.Get("Please wait"));
			object error = null;
			string purchaseSessionId = null;
			string transactionId = null;
			InAppPurchaseStatus status = InAppPurchaseStatus.Cancelled;
			yield return base.Purchase(product, delegate(object receivedError, string receivedPurchaseSessionId, string receivedTransactionId, InAppPurchaseStatus purchaseStatus)
			{
				error = receivedError;
				purchaseSessionId = receivedPurchaseSessionId;
				transactionId = receivedTransactionId;
				status = purchaseStatus;
			});
			float seconds = (float)(DateTime.UtcNow - time0).TotalMilliseconds * 0.001f;
			float min = 2f;
			if (seconds < min)
			{
				yield return FiberHelper.Wait(min - seconds, (FiberHelper.WaitFlag)0);
			}
			this.dialogProvider.CloseView(progressView);
			if (status == InAppPurchaseStatus.Failed)
			{
				string title = L.Get("In-app purchase failed");
				string message = L.Get("It was not possible to complete in-app purchase at this time");
				this.dialogProvider.ShowMessageBox(title, message, L.Get("ok"), null);
			}
			else if (status == InAppPurchaseStatus.Initiated)
			{
				string title2 = L.Get("In-app purchase initiated");
				string message2 = L.Get("You will receive your product when payment completes.");
				this.dialogProvider.ShowMessageBox(title2, message2, L.Get("ok"), null);
			}
			if (callback != null)
			{
				callback(purchaseSessionId, transactionId, status);
			}
			yield break;
		}

		public IEnumerator RestoreAppPurchases(Action<bool> callback)
		{
			DateTime time0 = DateTime.UtcNow;
			object progressView = this.dialogProvider.ShowProgressView(L.Get("Please wait"));
			object error = null;
			yield return base.RestoreTransactions(delegate(object receivedError)
			{
				error = receivedError;
			});
			float seconds = (float)(DateTime.UtcNow - time0).TotalMilliseconds * 0.001f;
			float min = 2f;
			if (seconds < min)
			{
				yield return FiberHelper.Wait(min - seconds, (FiberHelper.WaitFlag)0);
			}
			this.dialogProvider.CloseView(progressView);
			if (error != null)
			{
				string title = L.Get("Restore Purchases Failed");
				string description = L.Get("It was not possible to restore in-app purchases at this time");
				object msgView = this.dialogProvider.ShowMessageBox(title, description, L._("Ok"), null);
				yield return this.dialogProvider.WaitForClosingView(msgView);
				if (callback != null)
				{
					callback(false);
				}
			}
			else if (callback != null)
			{
				callback(true);
			}
			yield break;
		}

		private readonly IDialogViewProvider dialogProvider;

		[SettingsProvider("iap", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<InAppPurchaseManager.PersistableState>, IPersistableState
		{
			[JsonSerializable("pu", null)]
			public bool IsPayingUser { get; set; }

			public void MergeFromOther(InAppPurchaseManager.PersistableState newState, InAppPurchaseManager.PersistableState lastCloudState)
			{
				this.IsPayingUser = (this.IsPayingUser || newState.IsPayingUser || (lastCloudState != null && lastCloudState.IsPayingUser));
			}
		}
	}
}
