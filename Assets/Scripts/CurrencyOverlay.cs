using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class CurrencyOverlay : UIView
{
	protected override void ViewWillDisappear()
	{
		this.animFiber.Terminate();
	}

	public void WobbleCoinBar(float soundPitch = 1f)
	{
		this.animFiber.Start(this.Animate());
	}

	private IEnumerator Animate()
	{
		float dur = 0.4f;
		float scale = 1.4f;
		Vector3 scaleBegin = new Vector3(1f, 1f, 1f);
		Vector3 scaleEnd = new Vector3(scale, scale, 1f);
		yield return FiberAnimation.ScaleTransform(this.coinButton.transform, scaleBegin, scaleEnd, this.scaleCurve, dur);
		yield break;
	}

	private void CoinButtonClicked(UIEvent e)
	{
		if (this.OnCoinButtonClicked != null)
		{
			this.OnCoinButtonClicked();
		}
	}

	public CoinsAnimator coinAnimator;

	public CoinsButton coinButton;

	private Fiber animFiber = new Fiber();

	public AnimationCurve scaleCurve;

	public Action OnCoinButtonClicked;
}
