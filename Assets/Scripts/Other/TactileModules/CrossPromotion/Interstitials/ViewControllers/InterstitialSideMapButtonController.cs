using System;
using TactileModules.Ads;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.Interstitials.ViewFactories;
using TactileModules.CrossPromotion.Interstitials.Views;
using TactileModules.SideMapButtons;

namespace TactileModules.CrossPromotion.Interstitials.ViewControllers
{
	public class InterstitialSideMapButtonController : ISideMapButtonController
	{
		public InterstitialSideMapButtonController(ICrossPromotionAdRetriever interstitialAdRetriever, ICrossPromotionInterstitialViewFactory viewFactory, IInterstitialViewController viewController, IInterstitialPresenter interstitialPresenter)
		{
			this.interstitialAdRetriever = interstitialAdRetriever;
			this.viewController = viewController;
			this.interstitialPresenter = interstitialPresenter;
			this.button = viewFactory.CreateSideMapButton();
			this.button.Clicked += this.ButtonClicked;
			this.VisibilityChecker(null);
		}

		private ICrossPromotionAd PromotionAd
		{
			get
			{
				return this.interstitialAdRetriever.GetPresentablePromotion();
			}
		}

		private void ButtonClicked()
		{
			this.viewController.ShowViewIfPossible("MainMapButton");
			this.buttonHasBeenClicked = true;
		}

		public bool VisibilityChecker(object data)
		{
			if (!this.interstitialPresenter.InterstitialRequirementsAreMet())
			{
				return false;
			}
			if (this.PromotionAd != this.currentPromotionAd)
			{
				if (this.PromotionAd != null)
				{
					this.button.UpdateVisuals(this.PromotionAd);
				}
				this.currentPromotionAd = this.PromotionAd;
			}
			return !this.buttonHasBeenClicked && this.currentPromotionAd != null;
		}

		public ISideMapButton GetSideMapButtonInstance()
		{
			return this.button;
		}

		private readonly ICrossPromotionAdRetriever interstitialAdRetriever;

		private readonly IInterstitialViewController viewController;

		private readonly IInterstitialPresenter interstitialPresenter;

		private readonly IInterstitialSideMapButton button;

		private ICrossPromotionAd currentPromotionAd;

		private bool buttonHasBeenClicked;
	}
}
