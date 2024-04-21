using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class FlashAtCurrentLevel : MonoBehaviour
{
	private void OnEnable()
	{
		this.ResetAnimation();
	}

	private void OnDisable()
	{
		this.anim.Terminate();
	}

	private void Update()
	{
		this.anim.Step();
	}

	public void ResetAnimation()
	{
		this.anim.Start(this.FlashAnim());
	}

	private IEnumerator FlashAnim()
	{
		UISprite sprite = base.GetComponent<UISprite>();
		if (sprite == null)
		{
			yield break;
		}
		for (;;)
		{
			yield return FiberAnimation.Animate(this.duration, delegate(float t)
			{
				this.transform.localScale = UIWidget.DEFAULT_VECTOR3_ONE * (1f + t * this.growth);
				sprite.Alpha = 1f - t;
			});
		}
		yield break;
	}

	private readonly Fiber anim = new Fiber(FiberBucket.Manual);

	public float growth = 1f;

	public float duration = 1f;
}
