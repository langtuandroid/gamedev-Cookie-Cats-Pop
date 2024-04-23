using System;
using System.Collections;
using UnityEngine;

public class FrogHitEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		for (int i = 0; i < this.bubblesToHide.Length; i++)
		{
			this.bubblesToHide[i].SetActive(true);
		}
		this.frogSpine.PlayStartingFromEvent("FrogIntoBubble", "Hit");
		IEnumerator[] trailEffectAnimations = new IEnumerator[this.trailEffects.Length];
		for (int j = 0; j < this.trailEffects.Length; j++)
		{
			trailEffectAnimations[j] = this.trailEffects[j].Animate();
		}
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(this.scaler, Vector3.zero, Vector3.one, SingletonAsset<PowerVisualSettings>.Instance.frogHitEffectScaleCurve, 0f),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0),
				FiberAnimation.ShakeDecreasingLocalPosition(this.scaler, 0.6f, 2f, 120f, 0f)
			}),
			FiberHelper.RunSerial(new IEnumerator[]
			{
				FiberHelper.RunDelayed(1.13333333f, delegate
				{
					for (int k = 0; k < this.hitParticleEffects.Length; k++)
					{
						this.hitParticleEffects[k].Play();
					}
					for (int l = 0; l < this.bubblesToHide.Length; l++)
					{
						this.bubblesToHide[l].SetActive(false);
					}
				}),
				FiberHelper.RunParallel(new IEnumerator[]
				{
					FiberHelper.RunParallel(trailEffectAnimations),
					this.WaitAndFadeOut(0.6f)
				})
			})
		});
		yield return FiberHelper.Wait(0.3f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	private IEnumerator WaitAndFadeOut(float duration)
	{
		yield return FiberHelper.Wait(duration, (FiberHelper.WaitFlag)0);
		yield return FiberAnimation.ScaleTransform(this.scaler, this.scaler.localScale, Vector3.zero, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.3f);
		yield break;
	}

	public SkeletonAnimation frogSpine;

	public AnimationCurve fallingCurve;

	public Transform scaler;

	public ParticleSystem[] hitParticleEffects;

	public GameObject[] bubblesToHide;

	public FrogHitEffect.TrailEffect[] trailEffects;

	[Serializable]
	public class TrailEffect
	{
		public IEnumerator Animate()
		{
			this.particleSystem.Play();
			yield return FiberAnimation.MoveTransform(this.particleSystem.transform, this.particleSystem.transform.parent.TransformPoint(Vector3.zero), this.target.position, AnimationCurve.Linear(0f, 0f, 1f, 1f), this.duration);
			yield break;
		}

		public ParticleSystem particleSystem;

		public Transform target;

		public float duration = 0.5f;
	}
}
