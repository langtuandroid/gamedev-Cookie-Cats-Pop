using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class ThemeHuntRewardView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		List<ItemAmount> rewards = (List<ItemAmount>)parameters[0];
		this.rewardGrid.Initialize(rewards, true);
	}

	private IEnumerator ClaimRewards()
	{
		yield return this.rewardGrid.Animate(false, true);
		base.Close(1);
		yield break;
	}

	private void Claim(UIEvent e)
	{
		if (!this.claimed)
		{
			this.claimed = true;
			this.animationFiber.Start(this.ClaimRewards());
		}
	}

	[SerializeField]
	private RewardGrid rewardGrid;

	private bool claimed;

	private readonly Fiber animationFiber = new Fiber();
}
