using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class SlidesAndLaddersRewardAnimation : MonoBehaviour, IRewardAnimation
{
	public IEnumerator Animate()
	{
		this.presentSpine.state.SetAnimation(0, "open", false);
		yield return FiberHelper.Wait(this.chestOpenAnimationDelay, (FiberHelper.WaitFlag)0);
		this.presentSpine.state.AddAnimation(0, "open cycle", true, 0f);
		this.rewardGrid.gameObject.SetActive(true);
		yield return this.rewardGrid.Animate(false, true);
		yield break;
	}

	public void PlaySound()
	{
		SingletonAsset<SoundDatabase>.Instance.rewardChestOpen.Play();
	}

	public void Initialize(List<ItemAmount> rewards)
	{
		this.presentSpine.PlayAnimation(0, "rumble", true, false);
		this.rewardGrid.Initialize(rewards, true);
		this.rewardGrid.gameObject.SetActive(false);
	}

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private SkeletonAnimation presentSpine;

	[SerializeField]
	private float chestOpenAnimationDelay;
}
