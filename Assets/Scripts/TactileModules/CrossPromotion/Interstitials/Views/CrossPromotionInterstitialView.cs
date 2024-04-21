using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using UnityEngine;

namespace TactileModules.CrossPromotion.Interstitials.Views
{
	public class CrossPromotionInterstitialView : UIView, ICrossPromotionInterstitialView, IUIView
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ClickedAd;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action Closed;



		public void Initialize(ICrossPromotionAd crossPromotionAd)
		{
			this.image.SetTexture(crossPromotionAd.GetImage());
		}

		[UsedImplicitly]
		private void AdClicked(UIEvent e)
		{
			this.ClickedAd();
		}

		[UsedImplicitly]
		private void CloseView(UIEvent e)
		{
			this.Closed();
		}

		[SerializeField]
		private UITextureQuad image;
	}
}
