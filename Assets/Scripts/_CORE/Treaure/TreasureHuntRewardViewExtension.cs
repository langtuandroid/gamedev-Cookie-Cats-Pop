using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.PuzzleGames.TreasureHunt;
using UnityEngine;

public class TreasureHuntRewardViewExtension : MonoBehaviour, TreasureHuntRewardView.IExtension
{
	public void Initialize(List<ItemAmount> rewards)
	{
		SingletonAsset<SoundDatabase>.Instance.rewardChestOpen.Play();
		this.presentSpine.PlayAnimation(0, "rumble", true, false);
		this.rewardGrid.Initialize(rewards, true);
		this.rewardGrid.gameObject.SetActive(false);
	}

	public IEnumerator AnimateClaim()
	{
		this.presentSpine.state.SetAnimation(0, "open", false);
		yield return FiberHelper.Wait(this.chestOpenAnimationDelay, (FiberHelper.WaitFlag)0);
		this.presentSpine.state.AddAnimation(0, "open cycle", true, 0f);
		this.rewardGrid.gameObject.SetActive(true);
		yield return this.rewardGrid.Animate(false, true);
		yield break;
	}

	[SerializeField]
	private SkeletonAnimation presentSpine;

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private float chestOpenAnimationDelay;
}
