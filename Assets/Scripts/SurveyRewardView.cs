using System;
using System.Collections;
using Fibers;
using Tactile;
using UnityEngine;

public class SurveyRewardView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.rewardConfig = (parameters[0] as SurveyRewardData);
		this.label.text = L.Get(this.rewardConfig.Text);
		SingletonAsset<SoundDatabase>.Instance.rewardChestOpen.Play();
		this.ClaimButtonDisabled.gameObject.SetActive(false);
		this.ClaimButton.gameObject.SetActive(true);
		this.presentSpine.PlayAnimation(0, "Claim", true, false);
		this.rewardGrid.Initialize(this.rewardConfig.Items, true);
		this.rewardGrid.gameObject.SetActive(false);
	}

	protected override void ViewWillDisappear()
	{
		this.animationFiber.Terminate();
	}

	private void SetClaimButtonEnabled(bool enabled)
	{
		this.ClaimButtonDisabled.gameObject.SetActive(!enabled);
		this.ClaimButton.gameObject.SetActive(enabled);
	}

	private IEnumerator ClaimRewards()
	{
		UICamera.DisableInput();
		this.SetClaimButtonEnabled(false);
		foreach (ItemAmount itemAmount in this.rewardConfig.Items)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "Survey");
		}
		this.presentSpine.state.SetAnimation(0, "opening", false);
		yield return FiberHelper.Wait(this.chestOpenAnimationDelay, (FiberHelper.WaitFlag)0);
		this.presentSpine.state.AddAnimation(0, "open cycle", true, 0f);
		this.rewardGrid.gameObject.SetActive(true);
		yield return this.rewardGrid.Animate(false, true);
		base.Close(1);
		UICamera.EnableInput();
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
	private UIInstantiator ClaimButton;

	[SerializeField]
	private UIInstantiator ClaimButtonDisabled;

	[SerializeField]
	private SkeletonAnimation presentSpine;

	[SerializeField]
	private RewardGrid rewardGrid;

	[SerializeField]
	private float chestOpenAnimationDelay;

	[SerializeField]
	private UILabel label;

	private bool claimed;

	private readonly Fiber animationFiber = new Fiber();

	private SurveyRewardData rewardConfig;
}
