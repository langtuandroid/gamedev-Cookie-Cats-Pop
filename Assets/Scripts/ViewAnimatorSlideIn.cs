using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAnimatorSlideIn : UIViewAnimator
{
	private Transform AnimatingTransform
	{
		get
		{
			return (!this.animateParent || !(base.transform.parent != null)) ? base.transform : base.transform.parent;
		}
	}

	private void ViewWillAppear()
	{
		this.orgLocalPos = this.AnimatingTransform.localPosition;
		this.curve = SingletonAsset<ViewAnimatorCurves>.Instance.GetCurve(this.curveId);
	}

	private void ViewDidDisappear()
	{
		this.AnimatingTransform.localPosition = this.orgLocalPos;
	}

	public override IEnumerator AnimateIn()
	{
		Vector3 outOfView = Vector3.zero;
		outOfView = this.GetDestination(this.mode);
		this.AnimatingTransform.localPosition = outOfView;
		List<AnimatedButton> buttons = new List<AnimatedButton>(this.AnimatingTransform.gameObject.GetComponentsInChildren<AnimatedButton>());
		buttons.Sort(delegate(AnimatedButton a, AnimatedButton b)
		{
			float num = a.transform.position.x - a.transform.position.y;
			float num2 = b.transform.position.x - b.transform.position.y;
			if (num > num2)
			{
				return 1;
			}
			if (num < num2)
			{
				return -1;
			}
			return 0;
		});
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].PrepareAppearAnim();
		}
		FiberHelper.WaitFlag flags = (!this.skipOnTap) ? ((FiberHelper.WaitFlag)0) : FiberHelper.WaitFlag.StopOnMouseDown;
		yield return FiberHelper.Wait(this.delay, flags);
		for (int j = 0; j < buttons.Count; j++)
		{
			buttons[j].PlayAppearAnim(0.1f + (float)j * 0.05f);
		}
		if (this.curve != null)
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.Animate(0f, this.curve.In, delegate(float t)
				{
					this.AnimatingTransform.localPosition = FiberAnimation.LerpNoClamp(outOfView, this.orgLocalPos, t);
				}, false),
				FiberAnimation.Animate(0f, this.curve.InScale, delegate(float t)
				{
					this.AnimatingTransform.localScale = FiberAnimation.LerpNoClamp(Vector3.one, new Vector3(1.2f, 0.85f, 1f), t);
				}, false)
			});
		}
		yield break;
	}

	public override IEnumerator AnimateOut()
	{
		Vector3 dest = this.GetDestination(this.mode);
		if (this.curve != null)
		{
			yield return FiberHelper.RunParallel(new IEnumerator[]
			{
				FiberAnimation.Animate(0f, this.curve.Out, delegate(float t)
				{
					this.AnimatingTransform.localPosition = FiberAnimation.LerpNoClamp(this.orgLocalPos, dest, t);
				}, false),
				FiberAnimation.Animate(0f, this.curve.OutScale, delegate(float t)
				{
					this.AnimatingTransform.localScale = FiberAnimation.LerpNoClamp(Vector3.one, new Vector3(1.2f, 0.85f, 1f), t);
				}, false)
			});
		}
		yield break;
	}

	private Vector3 GetDestination(ViewAnimatorSlideIn.Mode m)
	{
		UICamera uicamera = UIViewManager.Instance.FindCameraFromObjectLayer(base.gameObject.layer);
		if (uicamera == null)
		{
			return Vector3.zero;
		}
		Camera cachedCamera = uicamera.cachedCamera;
		Vector3 vector = cachedCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
		Vector3 vector2 = cachedCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
		float num = vector2.x - vector.x;
		float num2 = vector2.y - vector.y;
		Vector3 b = Vector3.zero;
		switch (m)
		{
		case ViewAnimatorSlideIn.Mode.Up:
			b = Vector3.up * (num2 + 100f);
			break;
		case ViewAnimatorSlideIn.Mode.Down:
			b = Vector3.down * (num2 + 100f);
			break;
		case ViewAnimatorSlideIn.Mode.Left:
			b = Vector3.left * (num + 100f);
			break;
		case ViewAnimatorSlideIn.Mode.Right:
			b = Vector3.right * (num + 100f);
			break;
		}
		return this.AnimatingTransform.localPosition + b;
	}

	public ViewAnimatorSlideIn.Mode mode;

	public float delay;

	public bool skipOnTap;

	public string curveId;

	public bool animateParent;

	private Vector3 orgLocalPos;

	private ViewAnimatorCurves.Curve curve;

	public enum Mode
	{
		Up,
		Down,
		Left,
		Right
	}
}
