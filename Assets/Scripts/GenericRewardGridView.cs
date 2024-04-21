using System;
using System.Collections;
using System.Collections.Generic;

public class GenericRewardGridView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length == 0)
		{
			return;
		}
		this.rewards = (List<ItemAmount>)parameters[0];
	}

	protected override void ViewDidAppear()
	{
		if (this.rewards != null && this.rewards.Count > 0)
		{
			FiberCtrl.Pool.Run(this.ShowRewards(), false);
		}
		else
		{
			base.Close(0);
		}
	}

	private IEnumerator ShowRewards()
	{
		this.rewardGrid.Initialize(this.rewards, true);
		yield return this.rewardGrid.Animate(true, true);
		base.Close(0);
		yield break;
	}

	private void CloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	public RewardGrid rewardGrid;

	private List<ItemAmount> rewards;
}
