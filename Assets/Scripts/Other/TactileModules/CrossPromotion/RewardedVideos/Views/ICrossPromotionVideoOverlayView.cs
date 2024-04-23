using System;

namespace TactileModules.CrossPromotion.RewardedVideos.Views
{
	public interface ICrossPromotionVideoOverlayView : IUIView
	{
		UIElement GetFrame();

		void SetTimeLabel(float timeLeft);
	}
}
