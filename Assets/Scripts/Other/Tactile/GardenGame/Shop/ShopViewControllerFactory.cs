using System;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.Configuration;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewControllerFactory : IShopViewControllerFactory
	{
		public ShopViewControllerFactory(ShopManager shopManager, IUIController uiController, IAssetModel assets, ConfigGetter<ShopConfig> shopConfigProvider, ConfigGetter<LivesConfig> livesConfigProvider)
		{
			this.shopManager = shopManager;
			this.uiController = uiController;
			this.assets = assets;
			this.shopConfigProvider = shopConfigProvider;
			this.livesConfigProvider = livesConfigProvider;
		}

		public ShopViewController Create()
		{
			return new ShopViewController(this.shopManager, this.uiController, this.assets, this.shopConfigProvider, this.livesConfigProvider);
		}

		private readonly ShopManager shopManager;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly ConfigGetter<ShopConfig> shopConfigProvider;

		private readonly ConfigGetter<LivesConfig> livesConfigProvider;
	}
}
