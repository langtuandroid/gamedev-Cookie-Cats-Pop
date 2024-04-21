using System;
using TactileModules.CrossPromotion.Interstitials.Views;
using TactileModules.CrossPromotion.RewardedVideos.Views;

namespace TactileModules.CrossPromotion.TemplateAssets.GeneratedScript
{
	public interface IViewFactory
	{
		IInterstitialSideMapButton CreateCrossPromotionInterstitialSideMapButton();

		ICrossPromotionInterstitialView CreateCrossPromotionInterstitialView();

		ICrossPromotionVideoOverlayView CreateCrossPromotionVideoOverlayView();

		IRewardedVideoView CreateCrossPromotionVideoView();
	}
}
