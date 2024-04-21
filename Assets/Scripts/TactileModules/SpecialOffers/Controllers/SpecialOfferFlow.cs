using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffers.Views;

namespace TactileModules.SpecialOffers.Controllers
{
	public class SpecialOfferFlow : ISpecialOfferFlow
	{
		public SpecialOfferFlow(IViewPresenter placementViewMediator, ISpecialOfferViewFactory specialOfferViewFactory, IAnalyticsReporter analyticsReporter, ISpecialOffer offer, ISpecialOfferControllerFactory specialOfferControllerFactory, IInventoryManager inventoryManager, ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FlowStartedReason flowStartedReason)
		{
			this.placementViewMediator = placementViewMediator;
			this.offer = offer;
			this.specialOfferViewFactory = specialOfferViewFactory;
			this.specialOfferControllerFactory = specialOfferControllerFactory;
			this.inventoryManager = inventoryManager;
			this.analyticsReporter = analyticsReporter;
			this.specialOffersGlobalCoolDown = specialOffersGlobalCoolDown;
			analyticsReporter.LogSpecialOfferFlowStarted(this.offer.FeatureInstanceId, flowStartedReason);
		}

		public IEnumerator Run()
		{
			this.PrepareOffer();
			ISpecialOfferViewController viewController = this.specialOfferControllerFactory.CreateViewController(this.placementViewMediator, this.offer);
			EnumeratorResult<PurchaseData> purchaseDataResult = new EnumeratorResult<PurchaseData>();
			yield return viewController.ShowView(purchaseDataResult);
			if (purchaseDataResult.value.purchaseSuccessful)
			{
				this.GiveReward(this.offer.GetReward(), this.offer.FeatureInstanceId, purchaseDataResult);
				this.analyticsReporter.LogSpecialOfferDeactivated(this.offer.FeatureInstanceId, DeactivationReason.RewardGiven);
				yield return this.ShowRewardView();
				this.offer.Deactivate();
			}
			yield break;
		}

		private void PrepareOffer()
		{
			this.EnsureOfferIsActivated();
			this.SetTimeStamps();
		}

		private void EnsureOfferIsActivated()
		{
			if (!this.offer.IsActivated())
			{
				this.offer.Activate();
				this.analyticsReporter.LogSpecialOfferActivated(this.offer.FeatureInstanceId);
			}
		}

		private void SetTimeStamps()
		{
			this.offer.SetLastShowingTimeStamp();
			this.specialOffersGlobalCoolDown.Reset();
		}

		private void GiveReward(List<ItemAmount> rewards, string featureInstanceId, PurchaseData purchaseData)
		{
			foreach (ItemAmount itemAmount in rewards)
			{
				this.inventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, "SpecialOffer_" + featureInstanceId, purchaseData.purchaseSessionId, purchaseData.transactionId);
			}
		}

		private IEnumerator ShowRewardView()
		{
			ISpecialOfferRewardView view = this.specialOfferViewFactory.CreateRewardView(this.offer);
			this.placementViewMediator.ShowViewInstance<ISpecialOfferRewardView>(view, new object[0]);
			yield return view.AnimateClaimRewards();
			view.Close(1);
			yield break;
		}

		private readonly IViewPresenter placementViewMediator;

		private readonly ISpecialOffer offer;

		private readonly ISpecialOfferViewFactory specialOfferViewFactory;

		private readonly ISpecialOfferControllerFactory specialOfferControllerFactory;

		private readonly IInventoryManager inventoryManager;

		private readonly IAnalyticsReporter analyticsReporter;

		private readonly ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown;
	}
}
