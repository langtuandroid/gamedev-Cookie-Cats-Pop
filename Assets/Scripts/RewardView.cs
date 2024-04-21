using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

public class RewardView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.input = (parameters[0] as RewardView.Input);
		this.dialog.GetInstance<DialogFrame>().Title = this.input.title;
		this.rewardGrid.GetInstance<RewardGrid>().Initialize(this.input.rewards, true);
	}

	protected override void ViewDidAppear()
	{
		this.animFiber.Start(this.Animate());
	}

	private IEnumerator Animate()
	{
		yield return this.rewardGrid.GetInstance<RewardGrid>().Animate(false, true);
		base.Close(0);
		yield break;
	}

	public UIInstantiator dialog;

	public UIInstantiator rewardGrid;

	private Fiber animFiber = new Fiber();

	private RewardView.Input input;

	public class Input
	{
		public string title;

		public List<ItemAmount> rewards = new List<ItemAmount>();
	}
}
