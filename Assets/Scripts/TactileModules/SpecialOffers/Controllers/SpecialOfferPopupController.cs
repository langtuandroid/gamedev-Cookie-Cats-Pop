using System;
using System.Collections;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Controllers
{
	public class SpecialOfferPopupController : ISpecialOfferPopupController
	{
		public SpecialOfferPopupController(ISpecialOfferSelector specialOfferSelector, ISpecialOfferControllerFactory specialOfferControllerFactory, IExpiredOffersDeactivator expiredOffersDeactivator)
		{
			this.specialOfferSelector = specialOfferSelector;
			this.specialOfferControllerFactory = specialOfferControllerFactory;
			this.expiredOffersDeactivator = expiredOffersDeactivator;
		}

		public IEnumerator Run(IViewPresenter viewPresenter)
		{
			this.DeactivateExpiredOffers();
			yield return this.ShowOffer(viewPresenter);
			yield break;
		}

		private void DeactivateExpiredOffers()
		{
			this.expiredOffersDeactivator.DeactivateExpiredOffers();
		}

		private IEnumerator ShowOffer(IViewPresenter viewPresenter)
		{
			ISpecialOffer offer = this.specialOfferSelector.GetOffer();
			if (offer != null)
			{
				ISpecialOfferFlow flow = this.specialOfferControllerFactory.CreateFlow(viewPresenter, offer, FlowStartedReason.Automatic);
				yield return flow.Run();
			}
			yield break;
		}

		private readonly ISpecialOfferSelector specialOfferSelector;

		private readonly ISpecialOfferControllerFactory specialOfferControllerFactory;

		private readonly IExpiredOffersDeactivator expiredOffersDeactivator;
	}
}
