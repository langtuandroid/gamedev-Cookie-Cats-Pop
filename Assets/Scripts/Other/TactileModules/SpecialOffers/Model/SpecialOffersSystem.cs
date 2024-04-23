using System;
using TactileModules.SpecialOffers.Controllers;

namespace TactileModules.SpecialOffers.Model
{
	public class SpecialOffersSystem
	{
		public SpecialOffersSystem(SpecialOffersHandler featureHandler, IAvailableSpecialOffers availableSpecialOffers, ISpecialOfferControllerFactory specialOfferControllerFactory, ISpecialOfferPopupController popupController)
		{
			this.FeatureHandler = featureHandler;
			this.AvailableSpecialOffers = availableSpecialOffers;
			this.SpecialOfferControllerFactory = specialOfferControllerFactory;
			this.PopupController = popupController;
		}

		public SpecialOffersHandler FeatureHandler { get; private set; }

		public IAvailableSpecialOffers AvailableSpecialOffers { get; private set; }

		public ISpecialOfferControllerFactory SpecialOfferControllerFactory { get; private set; }

		public ISpecialOfferPopupController PopupController { get; private set; }
	}
}
