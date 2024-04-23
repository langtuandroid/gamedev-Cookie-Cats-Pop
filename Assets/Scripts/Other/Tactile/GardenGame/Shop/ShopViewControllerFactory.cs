using System;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.Configuration;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewControllerFactory : IShopViewControllerFactory
	{
		public ShopViewControllerFactory(ShopManager shopManager, InAppPurchaseManager inAppPurchaseManager, IUIController uiController, IAssetModel assets, ConfigGetter<ShopConfig> shopConfigProvider, ConfigGetter<LivesConfig> livesConfigProvider)
		{
			this.shopManager = shopManager;
			this.inAppPurchaseManager = inAppPurchaseManager;
			this.uiController = uiController;
			this.assets = assets;
			this.shopConfigProvider = shopConfigProvider;
			this.livesConfigProvider = livesConfigProvider;
		}

		public ShopViewController Create()
		{
			return new ShopViewController(this.shopManager, this.inAppPurchaseManager, this.uiController, this.assets, this.shopConfigProvider, this.livesConfigProvider);
		}

		private readonly ShopManager shopManager;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private readonly ConfigGetter<ShopConfig> shopConfigProvider;

		private readonly ConfigGetter<LivesConfig> livesConfigProvider;
	}
}
