using System;
using System.Collections;
using System.Collections.Generic;
using NinjaUI;
using UnityEngine;

public class OneTurnBoosterStampEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.stamp.SetActive(false);
		yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
		this.stamp.SetActive(true);
		yield return FiberHelper.RunParallel(new List<IEnumerator>
		{
			FiberAnimation.RotateTransform(this.stamp.transform, new Vector3(0f, 0f, -20f), new Vector3(0f, 0f, 0f), null, 0.2f),
			FiberAnimation.ScaleTransform(this.stamp.transform, Vector3.one * 8f, Vector3.one, null, 0.2f)
		}.ToArray());
		yield return CameraShaker.ShakeDecreasing(0.3f, 5f, 20f, 0f, false);
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		yield break;
	}

	[SerializeField]
	private GameObject stamp;
}
