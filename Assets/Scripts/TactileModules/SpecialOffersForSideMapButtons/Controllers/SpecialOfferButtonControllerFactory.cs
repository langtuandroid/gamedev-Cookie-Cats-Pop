using System;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGames.SpecialOffersForButtonArea;
using TactileModules.SideMapButtons;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Controllers;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffersForSideMapButtons.Controllers
{
	public class SpecialOfferButtonControllerFactory : ISideMapButtonControllerProvider
	{
		public SpecialOfferButtonControllerFactory(ISpecialOfferButtonTemplateAssetFactory templateAssetFactory, IAvailableSpecialOffers availableSpecialOffers, ISpecialOfferControllerFactory specialOfferControllerFactory, IUIViewManager uIViewManager)
		{
			this.templateAssetFactory = templateAssetFactory;
			this.availableSpecialOffers = availableSpecialOffers;
			this.specialOfferControllerFactory = specialOfferControllerFactory;
			this.uIViewManager = uIViewManager;
		}

		public List<ISideMapButtonController> CreateButtonControllers()
		{
			List<ISideMapButtonController> list = new List<ISideMapButtonController>();
			List<ISpecialOffer> offers = this.availableSpecialOffers.GetOffers();
			foreach (ISpecialOffer specialOffer in offers)
			{
				list.Add(new SpecialOfferButtonController(this.templateAssetFactory, specialOffer, this.specialOfferControllerFactory, this.uIViewManager, new Fiber()));
			}
			return list;
		}

		private readonly ISpecialOfferButtonTemplateAssetFactory templateAssetFactory;

		private readonly IAvailableSpecialOffers availableSpecialOffers;

		private readonly ISpecialOfferControllerFactory specialOfferControllerFactory;

		private readonly IUIViewManager uIViewManager;

		private readonly IAnalyticsReporter analyticsReporter;

		private readonly string featureInstanceId;
	}
}
