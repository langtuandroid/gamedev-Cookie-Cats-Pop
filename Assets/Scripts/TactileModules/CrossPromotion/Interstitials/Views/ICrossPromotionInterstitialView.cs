using System;
using TactileModules.CrossPromotion.General.Ads.AdModels;

namespace TactileModules.CrossPromotion.Interstitials.Views
{
	public interface ICrossPromotionInterstitialView : IUIView
	{
		event Action ClickedAd;

		event Action Closed;

		void Initialize(ICrossPromotionAd crossPromotionAd);
	}
}
