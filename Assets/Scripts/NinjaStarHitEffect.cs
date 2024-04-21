using System;
using System.Collections;
using UnityEngine;

public class NinjaStarHitEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.flashEffect.Play();
		IEnumerator[] enums = new IEnumerator[this.ninjaStars.Length * 2 + 1];
		enums[0] = FiberAnimation.RotateTransform(this.rotationPivot, new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, -270f), AnimationCurve.Linear(0f, 0f, 1f, 1f), 0.8f);
		float dAngle = 6.28318548f / (float)this.ninjaStars.Length;
		float radius = 100f;
		for (int i = 0; i < this.ninjaStars.Length; i++)
		{
			float f = dAngle * (float)i;
			enums[i + 1] = FiberAnimation.MoveLocalTransform(this.ninjaStars[i], Vector3.zero, new Vector3(Mathf.Sin(f) * radius, Mathf.Cos(f) * radius, 0f), AnimationCurve.Linear(0f, 0f, 1f, 1f), 0.3f);
		}
		for (int j = 0; j < this.ninjaStars.Length; j++)
		{
			float f2 = dAngle * (float)j;
			enums[j + 1 + this.ninjaStars.Length] = FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(0.6f, (FiberHelper.WaitFlag)0),
				FiberHelper.RunParallel(new IEnumerator[]
				{
					FiberAnimation.ScaleTransform(this.ninjaStars[j], this.ninjaStars[j].localScale, Vector3.zero, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.2f),
					FiberAnimation.MoveLocalTransform(this.ninjaStars[j], new Vector3(Mathf.Sin(f2) * radius, Mathf.Cos(f2) * radius, 0f), new Vector3(Mathf.Sin(f2) * radius * 2f, Mathf.Cos(f2) * radius * 2f, 0f), AnimationCurve.Linear(0f, 0f, 1f, 1f), 0.2f)
				})
			});
		}
		yield return FiberHelper.RunParallel(enums);
		yield break;
	}

	public Transform rotationPivot;

	public ParticleSystem flashEffect;

	public Transform[] ninjaStars;
}
