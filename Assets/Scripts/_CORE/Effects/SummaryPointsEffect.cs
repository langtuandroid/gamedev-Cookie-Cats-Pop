using System;
using System.Collections;
using UnityEngine;

public class SummaryPointsEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.label.text = parameters[0].ToString();
		this.text.text = parameters[2].ToString();
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(this.label.transform, Vector3.zero, Vector3.one, this.scale, 0f),
			FiberAnimation.MoveLocalTransform(this.label.transform, Vector3.zero, Vector3.up, this.drift, 0f),
			FiberAnimation.Animate(0f, this.fade, delegate(float progress)
			{
				this.label.Alpha = progress;
				this.text.Alpha = progress;
			}, false)
		});
		yield return null;
		yield break;
	}

	[SerializeField]
	private AnimationCurve scale;

	[SerializeField]
	private AnimationCurve drift;

	[SerializeField]
	private AnimationCurve fade;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private UILabel text;
}
