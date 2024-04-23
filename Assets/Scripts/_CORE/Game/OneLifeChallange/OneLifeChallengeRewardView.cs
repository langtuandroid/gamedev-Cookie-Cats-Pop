using System;
using System.Collections;
using Fibers;
using TactileModules.FeatureManager;
using UnityEngine;

public class OneLifeChallengeRewardView : UIView
{
	private OneLifeChallengeManager OneLifeChallengeManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<OneLifeChallengeManager>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		OneLifeChallengeConfig config = this.OneLifeChallengeManager.Config;
		SingletonAsset<SoundDatabase>.Instance.rewardChestOpen.Play();
		this.ClaimButtonDisabled.gameObject.SetActive(false);
		this.ClaimButton.gameObject.SetActive(true);
		this.presentSpine.PlayAnimation(0, "rumble", true, false);
		this.rewardGrid.Initialize(config.Rewards, true);
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
		this.presentSpine.state.SetAnimation(0, "open", false);
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

	private bool claimed;

	private readonly Fiber animationFiber = new Fiber();
}
