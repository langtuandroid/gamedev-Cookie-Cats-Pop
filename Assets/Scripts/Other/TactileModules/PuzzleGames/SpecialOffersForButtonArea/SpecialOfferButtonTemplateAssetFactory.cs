using System;
using TactileModules.SpecialOffersForSideMapButtons.SideMapButtons;
using UnityEngine;

namespace TactileModules.PuzzleGames.SpecialOffersForButtonArea
{
	public class SpecialOfferButtonTemplateAssetFactory : ISpecialOfferButtonTemplateAssetFactory
	{
		public ISpecialOfferSideMapButton CreateSpecialOfferSideButton()
		{
			SpecialOfferSideMapButton original = Resources.Load<SpecialOfferSideMapButton>("SpecialOffers/SpecialOfferSideButton");
			return UnityEngine.Object.Instantiate<SpecialOfferSideMapButton>(original);
		}
	}
}
