using System;
using System.Collections;
using TactileModules.PuzzleGames.LevelRush;
using UnityEngine;

[RequireComponent(typeof(LevelRushRewardView))]
public class LevelRushRewardViewExtension : MonoBehaviour, LevelRushRewardView.IExtension
{
	public void Initialize(LevelRushRewardView view)
	{
		LevelRushConfig.Reward reward = view.Reward;
		this.presentSpine.skeleton.SetSkin(this.GetSkinNameForReward(view.RewardIndex));
		this.rewardGrid.Initialize(reward.Items, true);
	}

	public IEnumerator AnimateClaim(LevelRushConfig.Reward reward)
	{
		yield return this.rewardGrid.Animate(true, true);
		yield break;
	}

	private string GetSkinNameForReward(int rewardIndex)
	{
		if (rewardIndex == 0)
		{
			return "Regular";
		}
		if (rewardIndex == 1)
		{
			return "Silver";
		}
		if (rewardIndex != 2)
		{
			return "Regular";
		}
		return "Gold";
	}

	[SerializeField]
	private SkeletonAnimation presentSpine;

	[SerializeField]
	private RewardGrid rewardGrid;
}
