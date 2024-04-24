using System;
using TactileModules.GameCore.ButtonArea;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Shop
{
	public static class ShopSystemBuilder
	{
		public static ShopSystem Build(FlowStack flowStack, IUIController uiController, IButtonAreaModel buttonAreaModel, IVisualInventory visualInventory, IConfigurationManager configurationManager, IUserSettings userSettings)
		{
			AssetModel assets = new AssetModel();
			ConfigGetter<ShopConfig> shopConfigProvider = new ConfigGetter<ShopConfig>(configurationManager);
			ConfigGetter<LivesConfig> livesConfigProvider = new ConfigGetter<LivesConfig>(configurationManager);
			ShopManagerProviderProxy shopManagerProviderProxy = new ShopManagerProviderProxy();
			ShopManager shopManager = ShopManager.CreateInstance(shopManagerProviderProxy, userSettings);
			ShopViewControllerFactory controllerFactory = new ShopViewControllerFactory(shopManager, uiController, assets, shopConfigProvider, livesConfigProvider);
			ShopViewFlowFactory shopViewFlowFactory = new ShopViewFlowFactory(controllerFactory, visualInventory);
			ShopManagerInterface implementation = new ShopManagerInterface();
			shopManagerProviderProxy.SetImplementation(implementation);
			CoinButtonHandler coinButtonHandler = new CoinButtonHandler(flowStack, buttonAreaModel, shopViewFlowFactory, assets, visualInventory);
			BuyShopItemFlowFactory buyShopItemFlowFactory = new BuyShopItemFlowFactory(shopManager, visualInventory, uiController, assets);
			return new ShopSystem(shopManager, shopViewFlowFactory, buyShopItemFlowFactory, coinButtonHandler);
		}
	}
}
