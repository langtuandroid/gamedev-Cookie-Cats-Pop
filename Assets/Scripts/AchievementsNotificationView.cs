using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class AchievementsNotificationView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		AchievementAsset achievementAsset = parameters[0] as AchievementAsset;
		this.title.text = L.Get("Achievement Unlocked");
		this.subtitle.text = achievementAsset.FormattedTitle;
		this.particles.Play();
		this.animFiber.Start(FiberHelper.RunParallel(new IEnumerator[]
		{
			this.Animate(),
			this.ColorCycle(this.title, 4f)
		}));
	}

	protected override void ViewWillAppear()
	{
		base.transform.parent.GetComponent<UIViewLayer>().ViewCamera.depth = 30f;
	}

	protected override void ViewUnload()
	{
		this.animFiber.Terminate();
	}

	private IEnumerator ColorCycle(UIWidget w, float duration)
	{
		while (duration > 0f)
		{
			float t = Time.timeSinceLevelLoad * 10f;
			w.Color = new Color(0.8f + Mathf.Sin((t + 10f) * 0.9f) * 0.2f, 0.8f + Mathf.Sin(t * -1.5f) * 0.2f, 0.8f + Mathf.Cos(t * 1.2f) * 0.2f, 1f);
			duration -= Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	private IEnumerator Animate()
	{
		yield return FiberAnimation.Animate(0.5f, this.curve, delegate(float t)
		{
		}, false);
		yield return FiberHelper.Wait(2.5f, (FiberHelper.WaitFlag)0);
		this.particles.Stop();
		yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
		base.Close(0);
		yield break;
	}

	public UILabel title;

	public UILabel subtitle;

	public ParticleSystem particles;

	private Fiber animFiber = new Fiber();

	public Transform slideIn;

	public AnimationCurve curve;

	public UIWidget icon;
}
