using System;
using System.Collections;
using Tactile;
using UnityEngine;

public class ReviewRewardView : UIView
{
	protected override void ViewWillAppear()
	{
		this.coinText.text = string.Format(L.Get("{0} Coins"), 5);
	}

	private IEnumerator AnimateRewardAndClose()
	{
		CurrencyOverlay currencyOverlay = UIViewManager.Instance.ObtainOverlay<CurrencyOverlay>(this);
		UICamera.DisableInput();
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		currencyOverlay.coinAnimator.GiveCoins(this.coinStartPivot.position, 5, 1f, null, null);
		InventoryManager.Instance.AddCoins(5, "review");
		yield return currencyOverlay.coinAnimator.WaitForAnimationToComplete();
		UICamera.EnableInput();
		UIViewManager.Instance.ReleaseOverlay<CurrencyOverlay>();
		base.Close(ReviewRewardView.Result.ReviewNow);
		yield break;
	}

	private void RateLaterButtonClicked(UIEvent e)
	{
		base.Close(ReviewRewardView.Result.ReviewLater);
	}

	private void RateNowButtonClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.AnimateRewardAndClose(), false);
	}

	public Transform coinStartPivot;

	public UILabel coinText;

	private const int NUM_COINS = 5;

	public enum Result
	{
		ReviewNow,
		ReviewLater
	}
}
