using System;
using TactileModules.PuzzleGames.LevelRush;
using UnityEngine;

[RequireComponent(typeof(LevelRushPresentInfoView))]
public class LevelRushPresentInfoViewExtension : MonoBehaviour, LevelRushPresentInfoView.IExtension
{
	public void Initialize(LevelRushPresentInfoView view)
	{
		LevelRushConfig.Reward reward = view.Reward;
		this.skeletonAnimation.skeleton.SetSkin(this.GetSkinNameForReward(view.RewardIndex));
		this.grid.Initialize(reward.Items, true);
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
	private RewardGrid grid;

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;
}
