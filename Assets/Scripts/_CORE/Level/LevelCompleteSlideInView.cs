using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class LevelCompleteSlideInView : UIView
{
	protected override void ViewDidAppear()
	{
		this.fiber.Start(this.Animate());
	}

	protected override void ViewWillAppear()
	{
		SingletonAsset<SoundDatabase>.Instance.goalAchieved.Play();
	}

	private IEnumerator Animate()
	{
		yield return FiberHelper.Wait(2f, FiberHelper.WaitFlag.StopOnMouseDown);
		if (this.particles != null)
		{
			this.particles.Stop();
		}
		base.Close(0);
		yield break;
	}

	private Fiber fiber = new Fiber();

	public ParticleSystem particles;
}
