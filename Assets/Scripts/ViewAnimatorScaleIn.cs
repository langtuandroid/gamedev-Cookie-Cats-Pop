using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class ViewAnimatorScaleIn : UIViewAnimator
{
	[UsedImplicitly]
	private void ViewWillAppear()
	{
		this.orgLocalScale = base.transform.localScale;
	}

	[UsedImplicitly]
	private void ViewDidDisappear()
	{
		base.transform.localScale = this.orgLocalScale;
	}

	public override IEnumerator AnimateIn()
	{
		base.transform.localScale = Vector3.zero;
		yield return FiberHelper.Wait(this.delay, (FiberHelper.WaitFlag)0);
		yield return FiberAnimation.Animate(0f, this.curve, delegate(float t)
		{
			base.transform.localScale = FiberAnimation.LerpNoClamp(this.startScale, Vector3.one, t);
		}, false);
		yield break;
	}

	public override IEnumerator AnimateOut()
	{
		yield break;
	}

	[SerializeField]
	private float delay;

	[SerializeField]
	private Vector3 startScale;

	[SerializeField]
	private AnimationCurve curve;

	private Vector3 orgLocalScale;
}
