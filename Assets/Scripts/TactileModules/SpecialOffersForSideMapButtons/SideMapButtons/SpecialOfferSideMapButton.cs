using System;
using TactileModules.SideMapButtons;
using UnityEngine;

namespace TactileModules.SpecialOffersForSideMapButtons.SideMapButtons
{
	public class SpecialOfferSideMapButton : SideMapButton, ISpecialOfferSideMapButton, ISideMapButton
	{
		public void SetTexture(Texture2D texture)
		{
			this.textureQuad.SetTexture(texture);
		}

		public void SetTimeLeft(string timeLeft)
		{
			this.timer.text = timeLeft;
		}

		[SerializeField]
		private UITextureQuad textureQuad;

		[SerializeField]
		private UILabel timer;
	}
}
