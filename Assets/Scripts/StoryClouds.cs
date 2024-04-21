using System;
using System.Collections;
using UnityEngine;

public class StoryClouds : MonoBehaviour
{
	public IEnumerator AnimateCloudsApart(float duration)
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.MoveLocalTransform(this.leftClouds, this.leftClouds.localPosition, this.leftClouds.localPosition + new Vector3(-800f, 0f, 0f), this.apartMovementCurve, duration),
			FiberAnimation.MoveLocalTransform(this.rightClouds, this.rightClouds.localPosition, this.rightClouds.localPosition + new Vector3(800f, 0f, 0f), this.apartMovementCurve, duration)
		});
		yield break;
	}

	public Transform leftClouds;

	public Transform rightClouds;

	public AnimationCurve apartMovementCurve;
}
