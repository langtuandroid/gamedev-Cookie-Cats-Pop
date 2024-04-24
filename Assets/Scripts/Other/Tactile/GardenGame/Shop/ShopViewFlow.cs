using System;
using System.Collections;
using Fibers;
using TactileModules.GameCore.Inventory;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewFlow : IFlow, IFiberRunnable
	{
		public ShopViewFlow(int coinsNeeded, IShopViewControllerFactory controllerFactory, IVisualInventory visualInventory)
		{
			this.coinsNeeded = coinsNeeded;
			this.controllerFactory = controllerFactory;
			this.visualInventory = visualInventory;
		}

		public IEnumerator Run()
		{
			this.shopViewFiber = new Fiber(FiberBucket.Manual);
			this.shopPurchaseFiber = new Fiber(FiberBucket.Manual);
			this.viewController = this.controllerFactory.Create();
			this.viewController.ItemClickedForPurchase += this.HandleItemClickedForPurchase;
			this.shopViewFiber.Start(this.viewController);
			while (this.shopViewFiber.Step())
			{
				this.shopPurchaseFiber.Step();
				yield return null;
			}
			yield break;
		}

		private void HandleItemClickedForPurchase(ShopViewItem item)
		{
		}



		private bool AnimatorFilter(InventoryItem item, int delta)
		{
			return item == "Coin" && delta > 0;
		}

		public void OnExit()
		{
		}

		private readonly int coinsNeeded;

		private readonly IShopViewControllerFactory controllerFactory;

		private readonly IVisualInventory visualInventory;

		private Fiber shopViewFiber;

		private Fiber shopPurchaseFiber;

		private ShopViewController viewController;
	}
}
