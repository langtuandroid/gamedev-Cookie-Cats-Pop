using System;
using System.Collections;
using UnityEngine;

public class FlyingInkEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.destinationPivot = (Transform)parameters[0];
		this.destination = new Vector3(this.destinationPivot.position.x + UnityEngine.Random.Range(-50f, 50f), this.destinationPivot.position.y, this.destinationPivot.position.z);
		Vector3 orgPos = base.transform.position;
		yield return FiberAnimation.Animate(0f, this.curve, delegate(float f)
		{
			Vector3 vector = FiberAnimation.LerpNoClamp(orgPos, this.destination, f);
			vector.z = orgPos.z;
			Vector3 vector2 = vector - this.transform.position;
			float angle = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			this.transform.rotation = rotation;
			this.transform.position = vector;
			this.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, this.scaleAmount, f);
		}, false);
		yield break;
	}

	[SerializeField]
	private AnimationCurve curve;

	[SerializeField]
	private float scaleAmount = 2f;

	private Transform destinationPivot;

	private Vector3 destination;
}
