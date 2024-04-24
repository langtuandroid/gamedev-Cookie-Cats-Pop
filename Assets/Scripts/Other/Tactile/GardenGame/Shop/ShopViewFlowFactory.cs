using System;
using TactileModules.GameCore.Inventory;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewFlowFactory : IShopViewFlowFactory
	{
		public ShopViewFlowFactory(IShopViewControllerFactory controllerFactory, IVisualInventory visualInventory)
		{
			this.controllerFactory = controllerFactory;
			this.visualInventory = visualInventory;
		}

		public ShopViewFlow CreateFlow(int coinsNeeded)
		{
			return new ShopViewFlow(coinsNeeded, this.controllerFactory, this.visualInventory);
		}

		private readonly IShopViewControllerFactory controllerFactory;

		private readonly IVisualInventory visualInventory;
		
	}
}
