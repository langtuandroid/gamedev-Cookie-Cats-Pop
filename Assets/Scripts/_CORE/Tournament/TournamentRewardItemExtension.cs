using System;
using System.Collections;
using UnityEngine;

public class TournamentRewardItemExtension : MonoBehaviour, TournamentRewardItem.IExtension
{
	public void InitializeRewardVisuals(TournamentPrizeConfig prizeTier)
	{
		this.rewardGrid.Clear();
		this.rewardGrid.Initialize(prizeTier.Rewards, false);
	}

	public IEnumerator AnimateGivingReward()
	{
		yield return this.rewardGrid.Animate(true, true);
		yield break;
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
