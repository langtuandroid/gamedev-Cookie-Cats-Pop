using System;
using System.Collections;
using UnityEngine;

public class CatPowerReadyEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		this.label.text = parameters[0].ToString();
		MatchFlag pieceColor = (MatchFlag)parameters[1];
		this.destinationPosition = (parameters[2] as Transform).position;
		base.transform.localScale = Vector3.one;
		if (pieceColor == string.Empty)
		{
			this.label.Color = Color.white;
		}
		for (int i = 0; i < this.numBlinks; i++)
		{
			yield return FiberAnimation.Animate(0f, this.blink, delegate(float f)
			{
				this.label.Alpha = f;
			}, false);
		}
		this.label.Alpha = 1f;
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			FiberAnimation.MoveTransform(base.transform, base.transform.position, this.destinationPosition, this.powercatReadyTravel, 0f),
			FiberAnimation.ScaleTransform(base.transform, Vector3.one, Vector3.one * 0.2f, this.powercatReadyTravel, 0f)
		});
		yield break;
	}

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private AnimationCurve blink;

	[SerializeField]
	private int numBlinks = 4;

	public AnimationCurve powercatReadyTravel;

	private Vector3 destinationPosition;
}
