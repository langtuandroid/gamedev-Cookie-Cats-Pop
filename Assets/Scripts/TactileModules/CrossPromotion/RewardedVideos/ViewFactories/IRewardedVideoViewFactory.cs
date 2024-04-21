using System;
using TactileModules.CrossPromotion.RewardedVideos.Views;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewFactories
{
	public interface IRewardedVideoViewFactory
	{
		IRewardedVideoView CreateRewardedVideoView();

		ICrossPromotionVideoOverlayView CreateRewardedVideoOverlayView();

		IUIView ShowSpinnerView();

		IUIView ShowVideoAbortedDialogView();
	}
}
