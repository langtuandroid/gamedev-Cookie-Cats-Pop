using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class GiveRewardsView : UIView
{
	public void Initialize(string title)
	{
		DialogFrame instance = this.dialogInstantiator.GetInstance<DialogFrame>();
		instance.Title = title;
	}

	public IEnumerator GiveRewardsAndClose(List<ItemAmount> itemAmounts)
	{
		yield return new Fiber.Wait(0.65f);
		RewardGrid rewardGrid = this.rewardGridInstantiator.GetInstance<RewardGrid>();
		rewardGrid.Initialize(itemAmounts, true);
		yield return rewardGrid.Animate(false, true);
		base.Close(0);
		yield break;
	}

	[SerializeField]
	private Instantiator rewardGridInstantiator;

	[SerializeField]
	private Instantiator dialogInstantiator;
}
