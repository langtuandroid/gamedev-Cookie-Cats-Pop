using System;
using TactileModules.GameCore.Inventory;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewFlowFactory : IShopViewFlowFactory
	{
		public ShopViewFlowFactory(IShopViewControllerFactory controllerFactory, IVisualInventory visualInventory, InAppPurchaseManager inAppPurchaseManager)
		{
			this.controllerFactory = controllerFactory;
			this.visualInventory = visualInventory;
			this.inAppPurchaseManager = inAppPurchaseManager;
		}

		public ShopViewFlow CreateFlow(int coinsNeeded)
		{
			return new ShopViewFlow(coinsNeeded, this.controllerFactory, this.visualInventory, this.inAppPurchaseManager);
		}

		private readonly IShopViewControllerFactory controllerFactory;

		private readonly IVisualInventory visualInventory;

		private readonly InAppPurchaseManager inAppPurchaseManager;
	}
}
