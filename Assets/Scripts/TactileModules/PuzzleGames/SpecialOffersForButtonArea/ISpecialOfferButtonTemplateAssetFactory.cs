using System;
using TactileModules.SpecialOffersForSideMapButtons.SideMapButtons;

namespace TactileModules.PuzzleGames.SpecialOffersForButtonArea
{
	public interface ISpecialOfferButtonTemplateAssetFactory
	{
		ISpecialOfferSideMapButton CreateSpecialOfferSideButton();
	}
}
