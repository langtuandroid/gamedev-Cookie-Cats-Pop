using System;
using System.Collections;
using UnityEngine;

public class ShopManagerInterface : ShopManager.IShopManagerInterface
{
	void ShopManager.IShopManagerInterface.LogCoinsSpentToAnalytics(ShopItem shopItem, object context)
	{
	}

	IEnumerator ShopManager.IShopManagerInterface.TrySpendingCoins(bool canAfford, ShopItem shopItem, object context)
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
			UIViewManager.UIViewStateGeneric<ShopView> shopViewState = UIViewManager.Instance.ShowView<ShopView>(new object[]
			{
				shopItem.CurrencyPrice
			});
			while (shopViewState.ClosingResult == null)
			{
				yield return null;
			}
		}
		else
		{
			SingletonAsset<UISetup>.Instance.purchaseSuccessful.Play();
			if (sender != null)
			{
				UICamera.DisableInput();
				CurrencyOverlay currencyOverlay = UIViewManager.Instance.FindOverlay<CurrencyOverlay>();
				currencyOverlay.coinButton.PauseRefreshingCoins(true);
				yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
				currencyOverlay.coinAnimator.UseCoins(sender.transform.position, (shopItem.CurrencyPrice <= 1) ? 1 : 5, 0.5f);
				yield return FiberHelper.Wait(0.6f, (FiberHelper.WaitFlag)0);
				currencyOverlay.coinButton.PauseRefreshingCoins(false);
				UICamera.EnableInput();
			}
		}
		yield break;
	}
}
