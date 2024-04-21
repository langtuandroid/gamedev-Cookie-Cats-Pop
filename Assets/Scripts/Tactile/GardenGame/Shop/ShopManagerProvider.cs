using System;
using System.Collections;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace Tactile.GardenGame.Shop
{
	public class ShopManagerProvider : ShopManager.IShopManagerInterface
	{
		public ShopManagerProvider(FlowStack flowStack, IShopViewFlowFactory shopViewFlowFactory)
		{
			this.flowStack = flowStack;
			this.shopViewFlowFactory = shopViewFlowFactory;
		}

		public IEnumerator TrySpendingCoins(bool canAfford, ShopItem shopItem, object context)
		{
			GameObject sender = context as GameObject;
			if (sender == null)
			{
				Component component = context as Component;
				if (component != null)
				{
					sender = component.gameObject;
				}
			}
			if (!canAfford)
			{
				yield return this.flowStack.PushAndWait(this.shopViewFlowFactory.CreateFlow(shopItem.CurrencyPrice));
			}
			else
			{
				SingletonAsset<UISetup>.Instance.purchaseSuccessful.Play();
				if (sender != null)
				{
				}
			}
			yield break;
		}

		public void LogCoinsSpentToAnalytics(ShopItem shopItem, object context)
		{
		}

		private readonly FlowStack flowStack;

		private readonly IShopViewFlowFactory shopViewFlowFactory;
	}
}
