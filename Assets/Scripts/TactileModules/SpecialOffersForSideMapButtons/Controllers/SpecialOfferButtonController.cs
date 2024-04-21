using System;
using Fibers;
using TactileModules.PuzzleGames.SpecialOffersForButtonArea;
using TactileModules.SideMapButtons;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Controllers;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffersForSideMapButtons.SideMapButtons;

namespace TactileModules.SpecialOffersForSideMapButtons.Controllers
{
	public class SpecialOfferButtonController : ISideMapButtonController
	{
		public SpecialOfferButtonController(ISpecialOfferButtonTemplateAssetFactory templateAssetFactory, ISpecialOffer specialOffer, ISpecialOfferControllerFactory specialOfferControllerFactory, IUIViewManager uIViewManager, IFiber fiber)
		{
			this.specialOffer = specialOffer;
			this.specialOfferControllerFactory = specialOfferControllerFactory;
			this.uIViewManager = uIViewManager;
			this.fiber = fiber;
			this.button = templateAssetFactory.CreateSpecialOfferSideButton();
			this.button.SetTexture(specialOffer.LoadSideMapButtonTexture());
			this.button.Clicked += this.ButtonClicked;
		}

		public bool VisibilityChecker(object data)
		{
			if (this.specialOffer.GetSecondsLeft() <= 0)
			{
				return false;
			}
			if (this.specialOffer.IsActivated())
			{
				this.UpdateTime();
				return true;
			}
			return false;
		}

		public ISideMapButton GetSideMapButtonInstance()
		{
			return this.button;
		}

		private void ButtonClicked()
		{
			if (!this.specialOffer.IsActivated())
			{
				return;
			}
			if (!this.fiber.IsTerminated)
			{
				return;
			}
			ISpecialOfferFlow specialOfferFlow = this.specialOfferControllerFactory.CreateFlow(this.uIViewManager, this.specialOffer, FlowStartedReason.UserClick);
			this.fiber.Start(specialOfferFlow.Run());
		}

		private void UpdateTime()
		{
			string timeLeft = this.specialOffer.GetTimeLeft();
			this.button.SetTimeLeft(timeLeft);
		}

		private readonly ISpecialOffer specialOffer;

		private readonly ISpecialOfferControllerFactory specialOfferControllerFactory;

		private readonly IUIViewManager uIViewManager;

		private readonly IFiber fiber;

		private readonly ISpecialOfferSideMapButton button;
	}
}
