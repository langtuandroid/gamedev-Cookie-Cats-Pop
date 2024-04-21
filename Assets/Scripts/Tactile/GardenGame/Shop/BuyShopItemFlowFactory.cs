using System;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;

namespace Tactile.GardenGame.Shop
{
	public class BuyShopItemFlowFactory : IBuyShopItemFlowFactory
	{
		public BuyShopItemFlowFactory(ShopManager shopManager, IVisualInventory visualInventory, IUIController uiController, IAssetModel assets)
		{
			this.shopManager = shopManager;
			this.visualInventory = visualInventory;
			this.uiController = uiController;
			this.assets = assets;
		}

		public BuyShopItemFlow CreateFlow(string shopItemIdentifier)
		{
			return new BuyShopItemFlow(shopItemIdentifier, this.shopManager, this.visualInventory, this.uiController, this.assets);
		}

		private readonly ShopManager shopManager;

		private readonly IVisualInventory visualInventory;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;
	}
}
