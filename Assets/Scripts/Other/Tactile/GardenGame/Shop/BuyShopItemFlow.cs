using System;
using System.Collections;
using Fibers;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Shop.Assets;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;

namespace Tactile.GardenGame.Shop
{
	public class BuyShopItemFlow : IFlow, IFiberRunnable
	{
		public BuyShopItemFlow(string shopItemIdentifier, ShopManager shopManager, IVisualInventory visualInventory, IUIController uiController, IAssetModel assets)
		{
			this.shopItemIdentifier = shopItemIdentifier;
			this.shopManager = shopManager;
			this.visualInventory = visualInventory;
			this.uiController = uiController;
			this.assets = assets;
		}

		public IEnumerator Run()
		{
			BuyShopItemView view = this.uiController.ShowView<BuyShopItemView>(this.assets.BuyShopItemView);
			this.SetupView(view);
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.AnimatorFilter)))
			{
				while (view.ClosingResult == null)
				{
					if (this.purchaseSucceeded)
					{
						yield return animator.Animate(view.buyButtonTransform.position);
						view.Close(0);
						yield break;
					}
					yield return null;
				}
				yield break;
			}
			yield break;
		}

		private void SetupView(BuyShopItemView view)
		{
			view.ItemPurchased += this.StartItemPurchase;
			ShopItem shopItem = this.shopManager.GetShopItem(this.shopItemIdentifier);
			ShopItemMetaData metaDataForShopItem = this.shopManager.GetMetaDataForShopItem(shopItem);
			string quantityText = string.Empty;
			if (shopItem.Rewards.Count == 1)
			{
				int amount = shopItem.Rewards[0].Amount;
				if (amount > 1)
				{
					InventoryItemMetaData metaData = this.visualInventory.InventoryManager.GetMetaData(shopItem.Rewards[0].ItemId);
					quantityText = metaData.FormattedQuantity(amount);
				}
			}
			view.Initialize(shopItem, metaDataForShopItem, quantityText);
		}

		private void StartItemPurchase()
		{
			this.shopManager.TrySpendCoins(this.shopItemIdentifier, null, new Action<bool>(this.HandlePurchaseResult));
		}

		private void HandlePurchaseResult(bool result)
		{
			this.purchaseSucceeded = result;
		}

		private bool AnimatorFilter(InventoryItem item, int delta)
		{
			return item == "Coin" && delta < 0;
		}

		public void OnExit()
		{
		}

		private readonly ShopManager shopManager;

		private readonly string shopItemIdentifier;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly IVisualInventory visualInventory;

		private bool purchaseSucceeded;
	}
}
