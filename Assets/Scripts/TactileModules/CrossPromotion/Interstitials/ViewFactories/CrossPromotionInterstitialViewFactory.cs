using System;
using TactileModules.CrossPromotion.Interstitials.Views;
using TactileModules.CrossPromotion.TemplateAssets.GeneratedScript;
using TactileModules.NinjaUi.SharedViewControllers;

namespace TactileModules.CrossPromotion.Interstitials.ViewFactories
{
	public class CrossPromotionInterstitialViewFactory : ICrossPromotionInterstitialViewFactory
	{
		public CrossPromotionInterstitialViewFactory(IViewFactory viewFactory, ISpinnerViewController spinnerViewController)
		{
			this.viewFactory = viewFactory;
			this.spinnerViewController = spinnerViewController;
		}

		public ICrossPromotionInterstitialView CreateInterstitialView()
		{
			return this.viewFactory.CreateCrossPromotionInterstitialView();
		}

		public IInterstitialSideMapButton CreateSideMapButton()
		{
			return this.viewFactory.CreateCrossPromotionInterstitialSideMapButton();
		}

		public IUIView ShowSpinnerView()
		{
			return this.spinnerViewController.ShowSpinnerView(L.Get("Please wait"));
		}

		private readonly IViewFactory viewFactory;

		private readonly ISpinnerViewController spinnerViewController;
	}
}
