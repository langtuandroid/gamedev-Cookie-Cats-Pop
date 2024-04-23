using System;
using System.Collections;
using UnityEngine;

public class FlyingMovesEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.label.text = parameters[0].ToString();
		this.destinationPivot = (Transform)parameters[1];
		Vector3 orgPos = base.transform.position;
		yield return FiberAnimation.Animate(0f, this.curve, delegate(float f)
		{
			Vector3 position = FiberAnimation.LerpNoClamp(orgPos, this.destinationPivot.position, f);
			position.z = orgPos.z;
			this.transform.position = position;
			this.transform.localScale = Vector3.one * Mathf.Lerp(this.scaleAmount, 1f, f);
		}, false);
		yield break;
	}

	[SerializeField]
	private AnimationCurve curve;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private float scaleAmount = 2f;

	private Transform destinationPivot;
}
