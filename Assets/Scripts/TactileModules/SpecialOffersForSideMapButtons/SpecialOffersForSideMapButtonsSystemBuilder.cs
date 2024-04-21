using System;
using TactileModules.PuzzleGames.SpecialOffersForButtonArea;
using TactileModules.SideMapButtons;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffersForSideMapButtons.Controllers;

namespace TactileModules.SpecialOffersForSideMapButtons
{
	public static class SpecialOffersForSideMapButtonsSystemBuilder
	{
		public static void Build(SpecialOffersSystem specialOffersSystem, SideMapButtonSystem sideMapButtonSystem, UIViewManager uIViewManager)
		{
			ISpecialOfferButtonTemplateAssetFactory templateAssetFactory = new SpecialOfferButtonTemplateAssetFactory();
			SpecialOfferButtonControllerFactory controllerProvider = new SpecialOfferButtonControllerFactory(templateAssetFactory, specialOffersSystem.AvailableSpecialOffers, specialOffersSystem.SpecialOfferControllerFactory, uIViewManager);
			sideMapButtonSystem.Registry.Register(controllerProvider);
		}
	}
}
