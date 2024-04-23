using System;
using UnityEngine;

namespace TactileModules.CrossPromotion.RewardedVideos.Views
{
	public class CrossPromotionVideoOverlayView : UIView, ICrossPromotionVideoOverlayView, IUIView
	{
		public UIElement GetFrame()
		{
			return this.frame;
		}

		public void SetTimeLabel(float timeLeft)
		{
			this.countDownLabel.text = Mathf.CeilToInt(timeLeft).ToString();
			this.countDownLabel.gameObject.SetActive(timeLeft > 0f);
		}

		[SerializeField]
		private UILabel countDownLabel;

		[SerializeField]
		private UIElement frame;
	}
}
