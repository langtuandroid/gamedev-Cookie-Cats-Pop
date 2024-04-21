using System;
using System.Collections;
using Fibers;
using TactileModules.GameCore.Inventory;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Shop
{
	public class ShopViewFlow : IFlow, IFiberRunnable
	{
		public ShopViewFlow(int coinsNeeded, IShopViewControllerFactory controllerFactory, IVisualInventory visualInventory, InAppPurchaseManager inAppPurchaseManager)
		{
			this.coinsNeeded = coinsNeeded;
			this.controllerFactory = controllerFactory;
			this.visualInventory = visualInventory;
			this.inAppPurchaseManager = inAppPurchaseManager;
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
			this.shopPurchaseFiber.Start(this.PurchaseCoins(item, this.coinsNeeded));
		}

		private IEnumerator PurchaseCoins(ShopViewItem item, int coinsNeeded)
		{
			EnumeratorResult<bool> purchaseSuccessful = new EnumeratorResult<bool>();
			ShopItem shopItem = item.GetShopItem();
			using (IInventoryItemAnimator coinAnimator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.AnimatorFilter)))
			{
				yield return this.DoInAppPurchase(shopItem, purchaseSuccessful);
				if (purchaseSuccessful.value)
				{
					SingletonAsset<UISetup>.Instance.purchaseSuccessful.Play();
					yield return coinAnimator.Animate(item.GetCoinSpawnPos());
					if (coinsNeeded > 0 && this.visualInventory.InventoryManager.Coins >= coinsNeeded)
					{
						this.viewController.CloseView();
					}
				}
			}
			yield break;
		}

		private IEnumerator DoInAppPurchase(ShopItem item, EnumeratorResult<bool> purchaseSuccessful)
		{
			InAppProduct product = this.inAppPurchaseManager.GetProductForIdentifier(item.FullIAPIdentifier);
			if (product == null)
			{
				yield break;
			}
			bool success = false;
			yield return this.inAppPurchaseManager.DoInAppPurchase(product, delegate(string receivedPurchaseSessionId, string receivedTransactionId, InAppPurchaseStatus resultStatus)
			{
				success = (resultStatus == InAppPurchaseStatus.Succeeded);
			});
			purchaseSuccessful.value = success;
			yield break;
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

		private readonly InAppPurchaseManager inAppPurchaseManager;

		private Fiber shopViewFiber;

		private Fiber shopPurchaseFiber;

		private ShopViewController viewController;
	}
}
