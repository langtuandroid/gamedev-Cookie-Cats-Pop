using System;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.SideMapButtons;

namespace TactileModules.CrossPromotion.Interstitials.Views
{
	public interface IInterstitialSideMapButton : ISideMapButton
	{
		void UpdateVisuals(ICrossPromotionAd crossPromotionAd);
	}
}
