using System;
using UnityEngine;

public static class AnimationCurveExtension
{
	public static float Duration(this AnimationCurve curve)
	{
		if (curve.keys.Length > 0)
		{
			return curve.keys[curve.length - 1].time;
		}
		return 0f;
	}

	public static float EvaluateNormalized(this AnimationCurve curve, float t)
	{
		return curve.Evaluate(t * curve.Duration());
	}
}
