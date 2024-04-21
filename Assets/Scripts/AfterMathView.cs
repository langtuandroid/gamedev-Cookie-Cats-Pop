using System;
using System.Collections;
using UnityEngine;

public class AfterMathView : UIView
{
	private IEnumerator GetBaseAnimateIn()
	{
		return base.AnimateIn();
	}

	public override IEnumerator AnimateIn()
	{
		this.Label.typingProgress = 0f;
		yield return this.GetBaseAnimateIn();
		yield return FiberAnimation.Animate(1f, delegate(float f)
		{
			this.Label.typingProgress = f;
		});
		yield break;
	}

	[Header("After Math View")]
	public UILabel Label;

	public float StayTime = 2f;
}
