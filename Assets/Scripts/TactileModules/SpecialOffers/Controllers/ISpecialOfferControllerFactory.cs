using System;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Controllers
{
	public interface ISpecialOfferControllerFactory
	{
		ISpecialOfferFlow CreateFlow(IViewPresenter placementViewMediator, ISpecialOffer specialOffer, FlowStartedReason flowStartedReason);

		ISpecialOfferViewController CreateViewController(IViewPresenter placementViewMediator, ISpecialOffer specialOffer);
	}
}
