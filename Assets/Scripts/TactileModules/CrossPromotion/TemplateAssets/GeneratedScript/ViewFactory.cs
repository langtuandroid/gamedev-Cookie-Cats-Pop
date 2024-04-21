using System;
using TactileModules.CrossPromotion.Interstitials.Views;
using TactileModules.CrossPromotion.RewardedVideos.Views;
using UnityEngine;

namespace TactileModules.CrossPromotion.TemplateAssets.GeneratedScript
{
	public class ViewFactory : IViewFactory
	{
		public IInterstitialSideMapButton CreateCrossPromotionInterstitialSideMapButton()
		{
			InterstitialSideMapButton original = Resources.Load<InterstitialSideMapButton>("CrossPromotion/CrossPromotionInterstitialSideMapButton");
			return UnityEngine.Object.Instantiate<InterstitialSideMapButton>(original);
		}

		public ICrossPromotionInterstitialView CreateCrossPromotionInterstitialView()
		{
			CrossPromotionInterstitialView original = Resources.Load<CrossPromotionInterstitialView>("CrossPromotion/CrossPromotionInterstitialView");
			return UnityEngine.Object.Instantiate<CrossPromotionInterstitialView>(original);
		}

		public ICrossPromotionVideoOverlayView CreateCrossPromotionVideoOverlayView()
		{
			CrossPromotionVideoOverlayView original = Resources.Load<CrossPromotionVideoOverlayView>("CrossPromotion/CrossPromotionVideoOverlayView");
			return UnityEngine.Object.Instantiate<CrossPromotionVideoOverlayView>(original);
		}

		public IRewardedVideoView CreateCrossPromotionVideoView()
		{
			RewardedVideoView original = Resources.Load<RewardedVideoView>("CrossPromotion/CrossPromotionVideoView");
			return UnityEngine.Object.Instantiate<RewardedVideoView>(original);
		}
	}
}
