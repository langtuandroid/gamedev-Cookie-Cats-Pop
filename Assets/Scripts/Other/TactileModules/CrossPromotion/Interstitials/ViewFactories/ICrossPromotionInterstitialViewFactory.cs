using System;
using TactileModules.CrossPromotion.Interstitials.Views;

namespace TactileModules.CrossPromotion.Interstitials.ViewFactories
{
	public interface ICrossPromotionInterstitialViewFactory
	{
		ICrossPromotionInterstitialView CreateInterstitialView();

		IInterstitialSideMapButton CreateSideMapButton();

		IUIView ShowSpinnerView();
	}
}
