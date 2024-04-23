using System;
using TactileModules.PuzzleGames.LevelRush;
using UnityEngine;

public class LevelRushPresentExtension : MonoBehaviour, LevelRushPresent.IExtension
{
	public void Initialize(LevelRushPresent present)
	{
		this.presentSpine.skeleton.SetSkin(this.GetSkinNameForReward(present.RewardIndex));
		this.present = present;
	}

	public void OnDropAnimationCompleted()
	{
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

	private LevelRushPresent present;
}
