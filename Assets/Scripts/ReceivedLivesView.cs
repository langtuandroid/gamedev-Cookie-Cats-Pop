using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class ReceivedLivesView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.numLives = (int)parameters[0];
		this.label.text = L.Get("You have received " + this.numLives + " lives!");
	}

	private void ClaimClicked(UIEvent e)
	{
		Fiber fiber = new Fiber();
		fiber.Start(this.AnimateLives());
	}

	public IEnumerator AnimateLives()
	{
		LivesOverlay livesOverlay = base.ObtainOverlay<LivesOverlay>();
		livesOverlay.heartAnimator.GiveCoins(this.LifeAnimationStartPivot.transform.position, this.numLives, 1f, null, null);
		yield return livesOverlay.heartAnimator.WaitForAnimationToComplete();
		base.ReleaseOverlay<LivesOverlay>();
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		base.Close(0);
		yield break;
	}

	public GameObject LifeAnimationStartPivot;

	public UILabel label;

	private int numLives;
}
