using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGame.ThemeHunt;
using UnityEngine;

public class ChristmasPresentMapThemeHuntItem : MapThemeHuntItem
{
	protected override void OnCollected()
	{
		this.animFiber.Start(this.anim());
		SingletonAsset<SoundDatabase>.Instance.plusTwo.Play();
	}

	private void Update()
	{
		if (!this.animFiber.IsTerminated)
		{
			this.animFiber.Step();
		}
	}

	private void OnEnable()
	{
		this.startScale = base.gameObject.transform.localScale;
	}

	private void OnDisable()
	{
		if (!this.animFiber.IsTerminated)
		{
			this.animFiber.Terminate();
		}
		base.gameObject.transform.localScale = this.startScale;
	}

	private IEnumerator anim()
	{
		yield return FiberAnimation.ScaleTransform(base.gameObject.transform, base.gameObject.transform.localScale, Vector3.zero, SingletonAsset<CommonCurves>.Instance.easeInOut, 0.3f);
		base.gameObject.SetActive(false);
		yield break;
	}

	private Fiber animFiber = new Fiber(FiberBucket.Manual);

	private Vector3 startScale;
}
