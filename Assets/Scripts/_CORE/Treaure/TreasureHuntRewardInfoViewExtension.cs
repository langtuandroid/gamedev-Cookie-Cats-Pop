using System;
using System.Collections.Generic;
using TactileModules.PuzzleGames.TreasureHunt;
using UnityEngine;

public class TreasureHuntRewardInfoViewExtension : MonoBehaviour, TreasureHuntRewardInfoView.IExtension
{
	public void InitializeRewardVisuals(List<ItemAmount> rewards)
	{
		this.rewardGrid.Initialize(rewards, true);
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
