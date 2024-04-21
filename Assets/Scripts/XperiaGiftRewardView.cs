using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using JetBrains.Annotations;
using UnityEngine;

public class XperiaGiftRewardView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		List<ItemAmount> rewards = parameters[0] as List<ItemAmount>;
		this.ClaimButton.gameObject.SetActive(true);
		this.rewardGrid.GetInstance<RewardGrid>().Initialize(rewards, true);
	}

	private IEnumerator ClaimRewards()
	{
		UICamera.DisableInput();
		this.ClaimButton.gameObject.SetActive(false);
		yield return this.rewardGrid.GetInstance<RewardGrid>().Animate(false, true);
		base.Close(1);
		UICamera.EnableInput();
		yield break;
	}

	[UsedImplicitly]
	public void Claim(UIEvent e)
	{
		if (this.claimed)
		{
			return;
		}
		this.claimed = true;
		this.animFiber.Start(this.ClaimRewards());
	}

	[SerializeField]
	private UIInstantiator rewardGrid;

	[SerializeField]
	private UIInstantiator ClaimButton;

	private readonly Fiber animFiber = new Fiber();

	private bool claimed;
}
