using System;
using System.Collections;
using UnityEngine;

public class AnimatingParticleEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.particles.Play();
		float maxDuration = 0f;
		ParticleSystem[] allSystems = this.particles.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < allSystems.Length; i++)
		{
			maxDuration = Mathf.Max(maxDuration, allSystems[i].main.startDelayMultiplier + allSystems[i].main.duration + allSystems[i].main.startLifetimeMultiplier);
		}
		yield return FiberHelper.Wait(maxDuration, (FiberHelper.WaitFlag)0);
		yield break;
	}

	public ParticleSystem particles;
}
