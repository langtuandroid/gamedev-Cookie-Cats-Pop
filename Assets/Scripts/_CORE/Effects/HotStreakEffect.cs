using System;
using System.Collections;
using UnityEngine;

public class HotStreakEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(this.label.transform, Vector3.zero, Vector3.one, this.scale, 0f),
			FiberAnimation.MoveLocalTransform(this.label.transform, Vector3.zero, Vector3.up * UnityEngine.Random.Range(1f, 3.5f), this.drift, 0f),
			FiberAnimation.Animate(0f, this.fade, delegate(float progress)
			{
				this.label.Alpha = progress;
			}, false)
		});
		yield break;
	}

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private AnimationCurve scale;

	[SerializeField]
	private AnimationCurve drift;

	[SerializeField]
	private AnimationCurve fade;
}
