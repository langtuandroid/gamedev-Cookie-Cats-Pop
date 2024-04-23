using System;
using TactileModules.SideMapButtons;
using UnityEngine;

namespace TactileModules.SpecialOffersForSideMapButtons.SideMapButtons
{
	public interface ISpecialOfferSideMapButton : ISideMapButton
	{
		void SetTexture(Texture2D texture);

		void SetTimeLeft(string timeLeft);
	}
}
