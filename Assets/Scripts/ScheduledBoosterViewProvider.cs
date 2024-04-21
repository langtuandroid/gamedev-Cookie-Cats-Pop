using System;
using System.Collections;
using TactileModules.PuzzleGame.ScheduledBooster.Views;
using UnityEngine;

public class ScheduledBoosterViewProvider : IScheduledBoosterViewProvider
{
	public IEnumerator AnimateCoins(UIView uiView, Vector3 buttonPos)
	{
		CurrencyOverlay currencyOverlay = UIViewManager.Instance.ObtainOverlay<CurrencyOverlay>(uiView);
		currencyOverlay.coinAnimator.UseCoins(buttonPos, 5, 0.5f);
		yield return currencyOverlay.coinAnimator.WaitForAnimationToComplete();
		if (uiView != null)
		{
			UIViewManager.Instance.ReleaseOverlay<CurrencyOverlay>();
		}
		yield break;
	}

	public IEnumerator AnimateCoinsBack(UIView uiView, Vector3 buttonPos)
	{
		CurrencyOverlay currencyOverlay = UIViewManager.Instance.ObtainOverlay<CurrencyOverlay>(uiView);
		currencyOverlay.coinAnimator.GiveCoins(buttonPos, 5, 0.5f, null, null);
		yield return currencyOverlay.coinAnimator.WaitForCoinsAndChangeLayerIfNeeded(uiView);
		if (uiView != null)
		{
			UIViewManager.Instance.ReleaseOverlay<CurrencyOverlay>();
		}
		yield break;
	}

	public IEnumerator ShowShopView()
	{
		yield return UIViewManager.Instance.ShowView<ShopView>(new object[]
		{
			0
		});
		yield break;
	}
}
