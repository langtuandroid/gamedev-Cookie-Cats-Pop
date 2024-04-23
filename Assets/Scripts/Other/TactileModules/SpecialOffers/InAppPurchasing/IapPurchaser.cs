using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.InAppPurchase;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.InAppPurchasing
{
	public class IapPurchaser : IIapPurchaser
	{
		public IapPurchaser(IInAppPurchaseManager inAppPurchaseManager, IFeatureManager featureManager, IFeatureTypeHandler specialOfferHandler, IPersistedPendingPurchases persistedPendingPurchases, IInventoryManager inventoryManager, IAnalyticsReporter analyticsReporter, IFiber fiber)
		{
			this.inAppPurchaseManager = inAppPurchaseManager;
			this.featureManager = featureManager;
			this.specialOfferHandler = specialOfferHandler;
			this.persistedPendingPurchases = persistedPendingPurchases;
			this.inventoryManager = inventoryManager;
			this.analyticsReporter = analyticsReporter;
			this.fiber = fiber;
			inAppPurchaseManager.PurchaseSuccessfulEvent += this.HandleIAPSuccess;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PurchaseData> OnPurchaseCompleted;



		public void Purchase(ISpecialOffer offer)
		{
			if (!this.fiber.IsTerminated)
			{
				return;
			}
			PendingPurchase pendingPurchase = this.CreatePendingPurchase(offer);
			this.persistedPendingPurchases.SetPendingPurchase(pendingPurchase);
			string iapidentifier = offer.GetIAPIdentifier();
			InAppProduct productForIdentifier = this.inAppPurchaseManager.GetProductForIdentifier(iapidentifier);
			this.fiber.Start(this.DoPurchase(productForIdentifier, pendingPurchase));
		}

		private IEnumerator DoPurchase(InAppProduct product, PendingPurchase pendingPurchase)
		{
			yield return this.inAppPurchaseManager.DoInAppPurchase(product, delegate(string receivedPurchaseSessionId, string receivedTransactionId, InAppPurchaseStatus resultStatus)
			{
				PurchaseData purchaseData = new PurchaseData
				{
					purchaseSessionId = receivedPurchaseSessionId,
					transactionId = receivedTransactionId
				};
				if (resultStatus == InAppPurchaseStatus.Succeeded)
				{
					this.DeletePersistedPendingPurchase(pendingPurchase);
					purchaseData.purchaseSuccessful = true;
					this.OnPurchaseCompleted(purchaseData);
				}
				else
				{
					purchaseData.purchaseSuccessful = false;
					this.OnPurchaseCompleted(purchaseData);
				}
			});
			yield break;
		}

		private PendingPurchase CreatePendingPurchase(ISpecialOffer offer)
		{
			return new PendingPurchase
			{
				Reward = offer.GetReward(),
				IAPIdentifier = offer.GetIAPIdentifier(),
				FeatureInstanceId = offer.FeatureInstanceId
			};
		}

		private void HandleIAPSuccess(InAppPurchaseManagerBase.PurchaseSuccessfulEventData purchaseSuccessfulEventData)
		{
			if (this.IsPurchasing())
			{
				return;
			}
			this.HandleIAPPurchaseWhenOutsideNormalFlow(purchaseSuccessfulEventData);
		}

		private bool IsPurchasing()
		{
			return !this.fiber.IsTerminated;
		}

		private void HandleIAPPurchaseWhenOutsideNormalFlow(InAppPurchaseManagerBase.PurchaseSuccessfulEventData purchaseSuccessfulEventData)
		{
			string productId = purchaseSuccessfulEventData.ProductId;
			PendingPurchase pendingPurchase = this.persistedPendingPurchases.GetPendingPurchase(productId);
			if (pendingPurchase == null)
			{
				return;
			}
			this.GiveReward(pendingPurchase.Reward, pendingPurchase.FeatureInstanceId, purchaseSuccessfulEventData.PurchaseSessionId, purchaseSuccessfulEventData.TransactionId);
			this.DeactivateOffer(pendingPurchase.FeatureInstanceId);
			this.DeletePersistedPendingPurchase(pendingPurchase);
		}

		private void DeletePersistedPendingPurchase(PendingPurchase pendingPurchase)
		{
			this.persistedPendingPurchases.DeletePendingPurchase(pendingPurchase);
		}

		private void GiveReward(List<ItemAmount> rewards, string featureInstanceId, string purchaseSessionId, string transactionId)
		{
			foreach (ItemAmount itemAmount in rewards)
			{
				this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "SpecialOffer_" + featureInstanceId, purchaseSessionId, transactionId);
			}
		}

		private void DeactivateOffer(string featureInstanceId)
		{
			ActivatedFeatureInstanceData activatedFeature = this.featureManager.GetActivatedFeature(this.specialOfferHandler, featureInstanceId);
			if (activatedFeature != null)
			{
				this.analyticsReporter.LogSpecialOfferDeactivated(activatedFeature.Id, DeactivationReason.IapPurchaseOutsideNormalFlow);
				this.featureManager.DeactivateFeature(this.specialOfferHandler, activatedFeature);
			}
		}

		private readonly IInAppPurchaseManager inAppPurchaseManager;

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler specialOfferHandler;

		private readonly IPersistedPendingPurchases persistedPendingPurchases;

		private readonly IInventoryManager inventoryManager;

		private readonly IAnalyticsReporter analyticsReporter;

		private readonly IFiber fiber;
	}
}
