using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class SlidesAndLaddersWheelRewardAnimation : MonoBehaviour, IRewardAnimation
{
	public IEnumerator Animate()
	{
		yield return FiberHelper.Wait(0.4f, (FiberHelper.WaitFlag)0);
		yield return this.rewardGrid.Animate(false, true);
		yield break;
	}

	public void Initialize(List<ItemAmount> rewards)
	{
		this.rewardGrid.Initialize(rewards, true);
	}

	public void PlaySound()
	{
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
