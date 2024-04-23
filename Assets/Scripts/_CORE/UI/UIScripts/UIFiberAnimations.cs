using System;
using System.Collections;
using UnityEngine;

public static class UIFiberAnimations
{
	public static IEnumerator FadeAlpha(UIWidget widget, float a, float b, float duration, AnimationCurve curve = null)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float f)
		{
			widget.Alpha = Mathf.Lerp(a, b, f);
		}, false);
		yield break;
	}

	public static IEnumerator AnimateColor(UIWidget widget, Color a, Color b, float duration, AnimationCurve curve = null)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float f)
		{
			widget.Color = Color.Lerp(a, b, f);
		}, false);
		yield break;
	}
}
