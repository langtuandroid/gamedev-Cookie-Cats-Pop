using System;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.SideMapButtons;
using UnityEngine;

namespace TactileModules.CrossPromotion.Interstitials.Views
{
	public class InterstitialSideMapButton : SideMapButton, IInterstitialSideMapButton, ISideMapButton
	{
		public void UpdateVisuals(ICrossPromotionAd crossPromotionAd)
		{
			this.textureQuad.SetTexture(crossPromotionAd.GetButtonImage());
		}

		public override SideMapButton.AreaSide Side
		{
			get
			{
				return SideMapButton.AreaSide.Right;
			}
		}

		[SerializeField]
		private UITextureQuad textureQuad;
	}
}
