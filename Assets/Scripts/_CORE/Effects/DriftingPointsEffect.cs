using System;
using System.Collections;
using UnityEngine;

public class DriftingPointsEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.label.text = parameters[0].ToString();
		MatchFlag pieceColor = (MatchFlag)parameters[1];
		this.label.fontStyle = SingletonAsset<LevelVisuals>.Instance.GetFontStyleFromMatchColor(pieceColor);
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.ScaleTransform(this.label.transform, Vector3.zero, Vector3.one, this.scale, 0f),
			FiberAnimation.MoveLocalTransform(this.label.transform, Vector3.zero, Vector3.up * UnityEngine.Random.Range(1f, 3.5f), this.drift, 0f),
			FiberAnimation.Animate(0f, this.fade, delegate(float progress)
			{
				this.label.Alpha = progress;
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
}
