using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

public class SlidesAndLaddersRewardInfoGrid : MonoBehaviour, IReward
{
	public void Initialize(List<ItemAmount> rewards)
	{
		this.rewardGrid.Initialize(rewards, true);
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
