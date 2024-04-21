using System;
using TactileModules.CrossPromotion.General.Ads.AdModels;

namespace TactileModules.CrossPromotion.RewardedVideos.Views
{
	public interface IRewardedVideoView : IUIView
	{
		event Action ClickedAd;

		event Action Closed;

		event Action OnVideoEnded;

		event Action<int> OnVideoFreeze;

		void Initialize(ICrossPromotionAd crossPromotionAd, ICrossPromotionVideoOverlayView overlayVideoView);
	}
}
