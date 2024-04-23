using System;
using System.Collections;
using Tactile;
using TactileModules.PuzzleGame.PiggyBank.Interfaces;

namespace TactileModules.PuzzleGame.PiggyBank.Controllers
{
	public class PiggyBankIAPController
	{
		public PiggyBankIAPController(InAppPurchaseManager inAppPurchaseManager, IIAPProvider iapProvider)
		{
			this.iapProvider = iapProvider;
			this.inAppPurchaseManager = inAppPurchaseManager;
			inAppPurchaseManager.PurchaseSuccessfulEvent += this.HandleIAPSuccess;
		}

		public IEnumerator Buy(InAppProduct product)
		{
			InAppPurchaseStatus inAppPurchaseStatus = InAppPurchaseStatus.Cancelled;
			if (product != null)
			{
				yield return this.inAppPurchaseManager.DoInAppPurchase(product, delegate(string resultPurchaseSessionId, string resultTransactionId, InAppPurchaseStatus resultStatus)
				{
					inAppPurchaseStatus = resultStatus;
				});
			}
			if (inAppPurchaseStatus == InAppPurchaseStatus.Cancelled && this.OnInAppPurchaseCancelled != null)
			{
				this.OnInAppPurchaseCancelled();
				this.Unsubscribe();
			}
			yield break;
		}

		private void HandleIAPSuccess(InAppPurchaseManagerBase.PurchaseSuccessfulEventData purchaseSuccessfulEventData)
		{
			if (this.IsOfferPurchase(purchaseSuccessfulEventData.ProductId))
			{
				if (this.OnClaimedOffer != null)
				{
					this.OnClaimedOffer();
				}
			}
			else if (this.IsOpenPurchase(purchaseSuccessfulEventData.ProductId) && this.OnClaimedContent != null)
			{
				this.OnClaimedContent();
			}
			this.Unsubscribe();
		}

		private bool IsOfferPurchase(string productId)
		{
			return productId == this.iapProvider.GetOfferInAppProduct().Identifier;
		}

		private bool IsOpenPurchase(string productId)
		{
			return productId == this.iapProvider.GetBuyOpenInAppProduct().Identifier;
		}

		private void Unsubscribe()
		{
			this.inAppPurchaseManager.PurchaseSuccessfulEvent -= this.HandleIAPSuccess;
		}

		public PiggyBankIAPController.InAppPurchaseCancelledEvent OnInAppPurchaseCancelled;

		public PiggyBankIAPController.ClaimedOfferEvent OnClaimedOffer;

		public PiggyBankIAPController.ClaimedContentEvent OnClaimedContent;

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private readonly IIAPProvider iapProvider;

		public delegate void InAppPurchaseCancelledEvent();

		public delegate void ClaimedOfferEvent();

		public delegate void ClaimedContentEvent();
	}
}
