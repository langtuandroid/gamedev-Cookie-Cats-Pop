using System;
using System.Collections;

namespace Tactile.GardenGame.Shop
{
	public class ShopManagerProviderProxy : ShopManager.IShopManagerInterface
	{
		public void SetImplementation(ShopManager.IShopManagerInterface implementation)
		{
			this.implementation = implementation;
		}

		public IEnumerator TrySpendingCoins(bool canAfford, ShopItem shopItem, object context)
		{
			yield return this.implementation.TrySpendingCoins(canAfford, shopItem, context);
			yield break;
		}

		public void LogCoinsSpentToAnalytics(ShopItem shopItem, object context)
		{
			this.implementation.LogCoinsSpentToAnalytics(shopItem, context);
		}

		private ShopManager.IShopManagerInterface implementation;
	}
}
