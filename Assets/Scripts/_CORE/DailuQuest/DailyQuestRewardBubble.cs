using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class DailyQuestRewardBubble : MonoBehaviour
{
	public void Initialize(DailyQuestInfo dayItem)
	{
		this.rewardGrid.Initialize(dayItem.Rewards, true);
		base.gameObject.SetActive(false);
		base.transform.localScale = Vector3.zero;
	}

	private void OnDestroy()
	{
		this.visibleFiber.Terminate();
	}

	public bool Visible
	{
		get
		{
			return this.visible;
		}
		set
		{
			if (this.visible != value)
			{
				this.visible = value;
				this.visibleFiber.Start(this.AnimateVisible(value));
			}
		}
	}

	private IEnumerator AnimateVisible(bool show)
	{
		if (show)
		{
			base.gameObject.SetActive(true);
		}
		if (show)
		{
			yield return FiberAnimation.ScaleTransform(base.transform, base.transform.localScale, Vector3.one, SingletonAsset<DailyQuestCurves>.Instance.rewardPopInCurve, 0f);
		}
		else
		{
			yield return FiberAnimation.ScaleTransform(base.transform, base.transform.localScale, Vector3.zero, SingletonAsset<DailyQuestCurves>.Instance.rewardHideCurve, 0f);
		}
		if (!show)
		{
			base.gameObject.SetActive(false);
		}
		yield break;
	}

	public IEnumerator AnimateRewards()
	{
		yield return this.rewardGrid.Animate(false, false);
		yield break;
	}

	public RewardGrid rewardGrid;

	private bool visible;

	private Fiber visibleFiber = new Fiber();
}
