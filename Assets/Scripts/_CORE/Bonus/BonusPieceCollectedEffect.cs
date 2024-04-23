using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using Spine;
using UnityEngine;

public class BonusPieceCollectedEffect : SpawnedEffect
{
	protected override IEnumerator AnimationLogic(object[] parameters)
	{
		if (BonusPieceCollectedEffect.isShowing)
		{
			yield return null;
			yield break;
		}
		BonusPieceCollectedEffect.isShowing = true;
		yield return new Fiber.OnExit(delegate()
		{
			BonusPieceCollectedEffect.isShowing = false;
		});
		MatchFlag color = (MatchFlag)parameters[0];
		Vector3 cookiePosition = (parameters.Length <= 1) ? base.transform.position : ((Vector3)parameters[1]);
		this.label.Alpha = 0f;
		this.label.text = SingletonAsset<FortuneCatSettings>.Instance.GetRandomMessageLocalized();
		this.counter.text = string.Format("{0}/{1}", Singleton<BonusDropManager>.Instance.DropsCollected, Singleton<BonusDropManager>.Instance.DropsRequiredForPrize);
		SingletonAsset<SoundDatabase>.Instance.fortuneCookieCollected.Play();
		Fiber fiber = new Fiber(FiberHelper.RunParallel(new IEnumerator[]
		{
			this.Effect(color, cookiePosition),
			FiberAnimation.ScaleTransform(this.counterPivot, Vector3.zero, Vector3.one, this.counterScaleCurve, 0f)
		}), FiberBucket.Manual);
		while (fiber.Step())
		{
			if (Input.GetMouseButtonUp(0))
			{
				break;
			}
			yield return null;
		}
		while (fiber.Step())
		{
		}
		yield return null;
		yield break;
	}

	private IEnumerator Effect(MatchFlag color, Vector3 cookiePosition)
	{
		Vector3 p = base.transform.position;
		p.z = ZLayer.EffectOverlay.Z();
		base.transform.position = p;
		this.spine.Skeleton.SetSkin(this.colorToSkin[color]);
		this.spine.PlayAnimation(0, "animation", false, true);
		TrackEntry anim = this.spine.state.GetCurrent(0);
		anim.Time = 0.13333334f;
		cookiePosition.z = base.transform.position.z;
		yield return FiberAnimation.MoveTransform(base.transform, cookiePosition, base.transform.position, null, 0.4f);
		Slot slot = this.spine.Skeleton.FindSlot("fortune_txt");
		float waitTime = anim.EndTime - 0.3f;
		while (waitTime > 0f)
		{
			this.label.Alpha = slot.a;
			waitTime -= Time.deltaTime;
			yield return null;
		}
		yield break;
	}

	[SerializeField]
	private SkeletonAnimation spine;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private AnimationCurve counterScaleCurve;

	[SerializeField]
	private Transform counterPivot;

	[SerializeField]
	private UILabel counter;

	private static bool isShowing;

	private Dictionary<MatchFlag, string> colorToSkin = new Dictionary<MatchFlag, string>
	{
		{
			"Blue",
			"blue"
		},
		{
			"Green",
			"green"
		},
		{
			"Red",
			"red"
		},
		{
			"Purple",
			"pink"
		},
		{
			"Yellow",
			"yellow"
		}
	};
}
