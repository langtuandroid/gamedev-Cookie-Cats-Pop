using System;
using System.Collections;
using UnityEngine;

public class ViewLayerSlideInAnimation : UIViewLayerAnimation
{
	public override IEnumerator AnimateIn(IUIView view)
	{
		Vector3 outOfView = Vector3.zero;
		outOfView = this.GetDestination(this.mode);
		view.transform.localPosition = outOfView;
		yield return FiberAnimation.Animate(0f, this.curveIn, delegate(float t)
		{
			view.transform.localPosition = FiberAnimation.LerpNoClamp(outOfView, this.orgLocalPos, t);
		}, false);
		yield break;
	}

	public override IEnumerator AnimateOut(IUIView view)
	{
		Vector3 dest = this.GetDestination(this.mode);
		yield return FiberAnimation.Animate(0f, this.curveOut, delegate(float t)
		{
			view.transform.localPosition = FiberAnimation.LerpNoClamp(this.orgLocalPos, dest, t);
		}, false);
		yield break;
	}

	private Vector3 GetDestination(ViewLayerSlideInAnimation.Mode m)
	{
		Vector3 result = Vector3.zero;
		switch (m)
		{
		case ViewLayerSlideInAnimation.Mode.Up:
			result = Vector3.up * 1000f;
			break;
		case ViewLayerSlideInAnimation.Mode.Down:
			result = Vector3.down * 1000f;
			break;
		case ViewLayerSlideInAnimation.Mode.Left:
			result = Vector3.left * 1000f;
			break;
		case ViewLayerSlideInAnimation.Mode.Right:
			result = Vector3.right * 1000f;
			break;
		}
		return result;
	}

	public AnimationCurve curveIn;

	public AnimationCurve curveOut;

	public ViewLayerSlideInAnimation.Mode mode;

	private Vector3 orgLocalPos = Vector3.zero;

	public enum Mode
	{
		Up,
		Down,
		Left,
		Right
	}
}
