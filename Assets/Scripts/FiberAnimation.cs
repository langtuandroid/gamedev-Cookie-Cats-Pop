using System;
using System.Collections;
using Fibers;
using UnityEngine;

public static class FiberAnimation
{
	public static bool FastForwardNextAnimation { get; set; }

	private static float Interpolate(float value, AnimationCurve curve)
	{
		return (curve != null && curve.length != 0) ? curve.Evaluate(value) : value;
	}

	public static Vector3 LerpNoClamp(Vector3 s, Vector3 d, float t)
	{
		return new Vector3(s.x + (d.x - s.x) * t, s.y + (d.y - s.y) * t, s.z + (d.z - s.z) * t);
	}

	public static IEnumerator Animate(float duration, AnimationCurve curve, Action<float> lerpFunc, bool shouldClampDeltaTime = false)
	{
		bool fastForward = FiberAnimation.FastForwardNextAnimation;
		FiberAnimation.FastForwardNextAnimation = false;
		float timer = 0f;
		float curveDuration = 1f;
		if (curve != null && curve.length > 0)
		{
			curveDuration = curve.keys[curve.length - 1].time;
			if (duration <= 0f)
			{
				duration = curveDuration;
			}
		}
		else if (duration <= 0f)
		{
			duration = 0.25f;
		}
		for (;;)
		{
			float whereOnCurve = Mathf.Clamp01(timer / duration) * curveDuration;
			float progress = FiberAnimation.Interpolate(whereOnCurve, curve);
			lerpFunc(progress);
			if (timer > duration)
			{
				break;
			}
			if (!shouldClampDeltaTime)
			{
				timer += Time.deltaTime;
			}
			else
			{
				timer += Mathf.Clamp(Time.deltaTime, 0f, 0.0333f);
			}
			if (!fastForward)
			{
				yield return null;
			}
		}
		yield break;
	}

	public static IEnumerator Animate(float duration, Action<float> lerpFunc)
	{
		return FiberAnimation.Animate(duration, null, lerpFunc, false);
	}

	public static IEnumerator ScaleTransform(Transform transform, Vector3 sourceScale, Vector3 destScale, AnimationCurve curve, float duration)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float lerpFactor)
		{
			transform.localScale = FiberAnimation.LerpNoClamp(sourceScale, destScale, lerpFactor);
		}, false);
		yield break;
	}

	public static IEnumerator MoveTransform(Transform transform, Vector3 sourcePosition, Vector3 destPosition, AnimationCurve curve, float duration)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float lerpFactor)
		{
			transform.position = FiberAnimation.LerpNoClamp(sourcePosition, destPosition, lerpFactor);
		}, false);
		yield break;
	}

	public static IEnumerator MoveLocalTransform(Transform transform, Vector3 sourcePosition, Vector3 destPosition, AnimationCurve curve, float duration)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float lerpFactor)
		{
			transform.localPosition = FiberAnimation.LerpNoClamp(sourcePosition, destPosition, lerpFactor);
		}, false);
		yield break;
	}

	public static IEnumerator RotateTransform(Transform transform, Vector3 sourceAngles, Vector3 destAngles, AnimationCurve curve, float duration)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float lerpFactor)
		{
			transform.localRotation = Quaternion.Euler(FiberAnimation.LerpNoClamp(sourceAngles, destAngles, lerpFactor));
		}, false);
		yield break;
	}

	public static IEnumerator RotateTransform(Transform transform, Quaternion source, Quaternion dest, AnimationCurve curve, float duration)
	{
		yield return FiberAnimation.Animate(duration, curve, delegate(float lerpFactor)
		{
			transform.localRotation = Quaternion.Slerp(source, dest, lerpFactor);
		}, false);
		yield break;
	}

	public static IEnumerator ShakeDecreasingLocalPosition(Transform transform, float duration = 0.3f, float amount = 10f, float tremor = 30f, float phase = 0f)
	{
		AnimationCurve curve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
		yield return FiberAnimation.ShakeLocalPosition(transform, transform.localPosition, curve, duration, amount, tremor, phase);
		yield break;
	}

	public static IEnumerator ShakeLocalPosition(Transform transform, Vector3 atPosition, AnimationCurve curve, float duration, float amount = 10f, float tremor = 30f, float phase = 0f)
	{
		yield return new Fiber.OnExit(delegate()
		{
			transform.localPosition = atPosition;
		});
		Vector3 p = Vector3.zero;
		yield return FiberAnimation.Animate(duration, curve, delegate(float t)
		{
			p.x = Mathf.Sin(t * tremor + phase) * t * amount;
			p.y = Mathf.Cos(t * tremor * 0.6f + phase) * t * amount;
			transform.localPosition = atPosition + p;
		}, false);
		yield break;
	}
}
