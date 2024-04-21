using System;
using System.Collections;
using UnityEngine;

public class SplashEffect : AnimatingEffect
{
	public override IEnumerator Animate()
	{
		yield return FiberAnimation.ScaleTransform(base.transform, Vector3.zero, Vector3.one, this.scaleCurve, 0f);
		yield break;
	}

	public AnimationCurve scaleCurve;
}
