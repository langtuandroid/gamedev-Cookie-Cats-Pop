using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.HotStreak.UI;
using UnityEngine;

public class CCPHotStreakProgressView : HotStreakProgressView
{
	protected override void ViewLoad(object[] parameters)
	{
		base.ViewLoad(parameters);
		int currentTierIndex = base.Manager.CurrentTierIndex;
		int num = Mathf.Min(base.Manager.Progress, base.Manager.MaxWins);
		string animationName = "Smoke";
		if (num > 0 && num <= base.Manager.MaxWins)
		{
			animationName = "Jug_Filling_" + num;
			this.smokeAnimation.gameObject.SetActive(true);
		}
		else
		{
			this.smokeAnimation.gameObject.SetActive(false);
		}
		if (this.spineAnimation.HasAnimation(animationName))
		{
			this.spineAnimation.AnimationState.SetAnimation(0, animationName, false);
		}
		this.SpawnRewards();
	}

	protected override IEnumerator PlayCurrentTierAnimation()
	{
		if (base.Manager.CurrentTierIndex >= 0)
		{
			UIGridLayout rewardGrid = this.rewardGrids[base.Manager.CurrentTierIndex];
			List<IEnumerator> scaleRoutines = new List<IEnumerator>();
			foreach (RewardItem rewardItem in rewardGrid.GetComponentsInChildren<RewardItem>())
			{
				scaleRoutines.Add(FiberAnimation.ScaleTransform(rewardItem.transform, Vector3.zero, Vector3.one, this.curve, 1f));
			}
			yield return FiberHelper.RunParallel(scaleRoutines.ToArray());
		}
		yield break;
	}

	private void SpawnRewards()
	{
		for (int i = 0; i < this.rewardGrids.Length; i++)
		{
			List<ItemAmount> bonusForTier = base.Manager.GetBonusForTier(i);
			RewardGrid component = this.rewardGrids[i].GetComponent<RewardGrid>();
			component.Initialize(bonusForTier, false);
		}
	}

	[SerializeField]
	private SkeletonAnimation spineAnimation;

	[SerializeField]
	private SkeletonAnimation smokeAnimation;

	[SerializeField]
	private float rewardAngle;

	private const string SmokeString = "Smoke";

	private const string JugFillingString = "Jug_Filling_";
}
