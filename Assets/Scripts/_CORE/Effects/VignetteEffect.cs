using System;
using System.Collections;
using UnityEngine;

public class VignetteEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		yield return UIFiberAnimations.FadeAlpha(this.vignette, 0f, 0.58431375f, (parameters.Length <= 0) ? 0f : ((float)parameters[0]), this.alphaCurve);
		yield break;
	}

	public UIWidget vignette;

	public AnimationCurve alphaCurve;
}
