using System;
using System.Collections;
using TactileModules.Placements;
using TactileModules.SpecialOffers.Controllers;

namespace TactileModules.SpecialOffers.Placements
{
	public class SpecialOffersPlacementRunnable : IPlacementRunnableNoBreak, IPlacementRunnable
	{
		public SpecialOffersPlacementRunnable(ISpecialOfferPopupController popupController)
		{
			this.popupController = popupController;
		}

		public string ID
		{
			get
			{
				return "AvailableSpecialOffers";
			}
		}

		public IEnumerator Run(IPlacementViewMediator placementViewMediator)
		{
			yield return this.popupController.Run(placementViewMediator);
			yield break;
		}

		private readonly ISpecialOfferPopupController popupController;
	}
}
