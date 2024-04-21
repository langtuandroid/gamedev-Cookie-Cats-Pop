using System;
using System.Collections;
using UnityEngine;

public class BurningParticleEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		Vector3 s = this.fire.transform.localScale;
		if (UnityEngine.Random.value < 0.5f)
		{
			s.x = -s.x;
			this.fire.transform.localScale = s;
		}
		this.particles.Play();
		float maxDuration = 0f;
		ParticleSystem[] allSystems = this.particles.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < allSystems.Length; i++)
		{
			maxDuration = Mathf.Max(maxDuration, allSystems[i].main.startDelayMultiplier + allSystems[i].main.duration + allSystems[i].main.startLifetimeMultiplier);
		}
		yield return FiberAnimation.Animate(maxDuration, this.alphaCurve, delegate(float t)
		{
			this.fire.skeleton.SetColor(new Color(1f, 1f, 1f, t));
		}, false);
		yield break;
	}

	public ParticleSystem particles;

	public AnimationCurve alphaCurve;

	public SkeletonAnimation fire;
}
