using System;
using Tactile;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffers.Views;

namespace TactileModules.SpecialOffers.Controllers
{
	public class SpecialOfferControllerFactory : ISpecialOfferControllerFactory
	{
		public SpecialOfferControllerFactory(ISpecialOfferViewFactory specialOfferViewFactory, ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, IShopManager shopManager, IInventoryManager inventoryManager, IAnalyticsReporter analyticsReporter)
		{
			this.specialOfferViewFactory = specialOfferViewFactory;
			this.specialOffersGlobalCoolDown = specialOffersGlobalCoolDown;
			this.shopManager = shopManager;
			this.inventoryManager = inventoryManager;
			this.analyticsReporter = analyticsReporter;
		}

		public ISpecialOfferViewController CreateViewController(IViewPresenter placementViewMediator, ISpecialOffer specialOffer)
		{
			SpecialOfferTypeEnum type = specialOffer.GetType();
			if (type == SpecialOfferTypeEnum.IAP)
			{
				return this.CreateIapViewController(placementViewMediator, specialOffer);
			}
			if (type == SpecialOfferTypeEnum.Coins)
			{
				return this.CreateCoinViewController(placementViewMediator, specialOffer);
			}
			if (type != SpecialOfferTypeEnum.Free)
			{
				throw new Exception("SpecialOfferViewController does not exist for type : " + specialOffer.GetType());
			}
			return this.CreateFreeViewController(placementViewMediator, specialOffer);
		}

		public ISpecialOfferFlow CreateFlow(IViewPresenter placementViewMediator, ISpecialOffer specialOffer, FlowStartedReason flowStartedReason)
		{
			return new SpecialOfferFlow(placementViewMediator, this.specialOfferViewFactory, this.analyticsReporter, specialOffer, this, this.inventoryManager, this.specialOffersGlobalCoolDown, flowStartedReason);
		}

		private ISpecialOfferViewController CreateIapViewController(IViewPresenter placementViewMediator, ISpecialOffer specialOffer)
		{
			return new SpecialOfferIapViewController(placementViewMediator, this.specialOfferViewFactory, this.analyticsReporter, specialOffer);
		}

		private ISpecialOfferViewController CreateCoinViewController(IViewPresenter placementViewMediator, ISpecialOffer specialOffer)
		{
			return new SpecialOfferCoinViewController(placementViewMediator, this.specialOfferViewFactory, this.analyticsReporter, specialOffer, this.shopManager);
		}

		private ISpecialOfferViewController CreateFreeViewController(IViewPresenter placementViewMediator, ISpecialOffer specialOffer)
		{
			return new SpecialOfferFreeViewController(placementViewMediator, this.specialOfferViewFactory, this.analyticsReporter, specialOffer);
		}

		private readonly ISpecialOfferViewFactory specialOfferViewFactory;

		private readonly ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown;

		private readonly IShopManager shopManager;

		private readonly IInventoryManager inventoryManager;

		private readonly IAnalyticsReporter analyticsReporter;
	}
}
