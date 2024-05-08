
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.Placements;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.SpecialOffers;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Controllers;
using TactileModules.SpecialOffers.Model;
using TactileModules.SpecialOffers.Placements;
using TactileModules.SpecialOffers.Views;
using TactileModules.TactilePrefs;

namespace TactileModules.SpecialOffers
{
	public static class SpecialOffersSystemBuilder
	{
		public static SpecialOffersSystem Build(IFeatureManager featureManager, ISpecialOffersMainProgressionProvider mainProgression, IConfigurationManager configurationManager, IPlacementRunnableRegistry placementSystemPlacementRunnableRegistry, IInventoryManager inventoryManager, IShopManager shopManager, PlacementIdentifier placementIdentifier)
		{
			string domainNamespace = "TactileModules.SpecialOffers.Model";
			string key = "PendingPurchases";
			SpecialOffersHandler specialOffersHandler = new SpecialOffersHandler(configurationManager);
			SpecialOffersGlobalCoolDown specialOffersGlobalCoolDown = new SpecialOffersGlobalCoolDown(featureManager, specialOffersHandler, configurationManager);
			AnalyticsReporter analyticsReporter = new AnalyticsReporter(featureManager, specialOffersHandler, specialOffersGlobalCoolDown);
			ITemplateAssetFactory templateAssetFactory = new TemplateAssetFactory();
			SpecialOfferViewFactory specialOfferViewFactory = new SpecialOfferViewFactory(templateAssetFactory);
			SpecialOfferControllerFactory specialOfferControllerFactory = new SpecialOfferControllerFactory(specialOfferViewFactory, specialOffersGlobalCoolDown, shopManager, inventoryManager, analyticsReporter);
			AvailableSpecialOffers availableSpecialOffers = new AvailableSpecialOffers(featureManager, specialOffersHandler);
			SpecialOfferSelector specialOfferSelector = new SpecialOfferSelector(mainProgression, availableSpecialOffers, specialOffersGlobalCoolDown, analyticsReporter);
			ExpiredOffersDeactivator expiredOffersDeactivator = new ExpiredOffersDeactivator(featureManager, specialOffersHandler, analyticsReporter);
			SpecialOfferPopupController popupController = new SpecialOfferPopupController(specialOfferSelector, specialOfferControllerFactory, expiredOffersDeactivator);
			SpecialOffersPlacementRunnable runnable = new SpecialOffersPlacementRunnable(popupController);
			placementSystemPlacementRunnableRegistry.RegisterRunnable(runnable, placementIdentifier, PlacementBehavior.Skippable);
			return new SpecialOffersSystem(specialOffersHandler, availableSpecialOffers, specialOfferControllerFactory, popupController);
		}
	}
}
