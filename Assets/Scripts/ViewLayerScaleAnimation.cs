using System;
using System.Collections;
using UnityEngine;

public class ViewLayerScaleAnimation : UIViewLayerAnimation
{
	public override IEnumerator AnimateIn(IUIView view)
	{
		Vector3 large = new Vector3(1.1f, 1.1f, 0.987f);
		view.transform.localScale = Vector3.zero;
		yield return FiberAnimation.Animate(0.1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			view.transform.localScale = FiberAnimation.LerpNoClamp(large, Vector3.one, t);
		}, false);
		yield break;
	}

	public override IEnumerator AnimateOut(IUIView view)
	{
		Vector3 small = new Vector3(0.9f, 0.9f, 0.987f);
		view.transform.localScale = Vector3.zero;
		yield return FiberAnimation.Animate(0.1f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), delegate(float t)
		{
			view.transform.localScale = FiberAnimation.LerpNoClamp(Vector3.one, small, t);
		}, false);
		yield break;
	}
}
