using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.SpecialOffers.Views;
using UnityEngine;

public class SpecialOfferRewardViewExtension : MonoBehaviour, ISpecialOfferRewardViewExtention
{
	public void Initialize(List<ItemAmount> rewards)
	{
		this.rewardGrid.Initialize(rewards, true);
	}

	public IEnumerator AnimateClaimRewards()
	{
		yield return this.rewardGrid.Animate(false, false);
		yield break;
	}

	[SerializeField]
	private RewardGrid rewardGrid;
}
