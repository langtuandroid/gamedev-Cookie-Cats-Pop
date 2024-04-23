using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

public class BoosterUnlockedView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.parameters = (BoosterUnlockedView.BoosterUnlockedViewParameters)parameters[0];
		this.continueIsClicked = false;
		BoosterMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>(this.parameters.itemUnlocked);
		this.celebrationState.SetActive(false);
		this.title.text = string.Format(L.Get("{0}!"), metaData.Title);
		this.rewardGrid.Initialize(this.parameters.rewards, true);
	}

	protected override void ViewDidAppear()
	{
		this.celebrationState.SetActive(true);
	}

	private void ContinueClicked(UIEvent e)
	{
		if (this.continueIsClicked)
		{
			return;
		}
		this.continueIsClicked = true;
		FiberCtrl.Pool.Run(this.RewardFlow(), false);
	}

	private IEnumerator RewardFlow()
	{
		base.Close(0);
		yield return this.rewardGrid.Animate(true, true);
		this.parameters.Completed();
		yield break;
	}

	public GameObject celebrationState;

	public RewardGrid rewardGrid;

	public UILabel title;

	private BoosterUnlockedView.BoosterUnlockedViewParameters parameters;

	private bool continueIsClicked;

	public class BoosterUnlockedViewParameters
	{
		public InventoryItem itemUnlocked;

		public List<ItemAmount> rewards;

		public Action Completed;
	}
}
