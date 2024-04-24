using System.Collections;
using System.Collections.Generic;
using Fibers;
using Tactile;
using UnityEngine;

public class VipProgramSubscriberView : UIView
{
	

	private DialogFrame Dialog
	{
		get
		{
			return this.dialog.GetInstance<DialogFrame>();
		}
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.UpdateUIFromState();
		this.box.Initialize();
	}

	protected override void ViewWillDisappear()
	{
		this.logicFiber.Terminate();
		this.animFiber.Terminate();
	}

	private void Update()
	{
		if (this.needUpdateUI)
		{
			this.UpdateUIFromState();
			this.needUpdateUI = false;
		}
	}

	private void UpdateUIFromState()
	{
		if (!VipManager.Instance.UserIsVip())
		{
			this.DismissView("vip program ends");
			return;
		}
		this.daysDescriptionPart1.text = VipManager.Instance.GetDaysLeft().ToString();
		this.AnimCalenderWobble();
		this.animFiber.Terminate();
		if (VipManager.Instance.IsBonusPending())
		{
			this.logicFiber.Start(this.RewardStateCr());
		}
		else
		{
			this.logicFiber.Start(this.WaitStateCr());
			this.animFiber.Start(this.AnimStanchionAppear());
		}
	}

	private IEnumerator WaitStateCr()
	{
		this.pivot_sync.SetActive(false);
		this.pivot_claim.SetActive(false);
		this.Dialog.HasCloseButton = true;
		this.pivot_wait.SetActive(true);
		while (!VipManager.Instance.IsBonusPending())
		{
			this.waitTimer.text = VipManager.Instance.GetSecondsLeftUntilNextBonusStr();
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		}
		this.needUpdateUI = true;
		yield break;
	}

	private IEnumerator RewardStateCr()
	{
		this.pivot_sync.SetActive(false);
		this.pivot_claim.SetActive(true);
		this.Dialog.HasCloseButton = false;
		this.pivot_wait.SetActive(false);
		yield return null;
		yield break;
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		this.DismissView("close");
	}

	private void ButtonClaimClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.ClaimReward(), false);
	}

	private void DismissView(string button)
	{
		base.Close(0);
	}

	private IEnumerator ClaimReward()
	{
		UICamera.DisableInput();
		List<ItemAmount> allBonuses = new List<ItemAmount>(VipManager.Instance.PendingBonus());
		List<ItemAmount> bonuses = allBonuses.GetMergedItems();
		if (bonuses.Count == 0)
		{
			yield break;
		}
		foreach (ItemAmount itemAmount in bonuses)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "VipBonus");
		}
		VipManager.Instance.ConsumePendingBonus();
		yield return this.AnimateBonus(bonuses);
		UICamera.EnableInput();
		this.needUpdateUI = true;
		yield break;
	}

	private IEnumerator AnimateBonus(List<ItemAmount> bonuses)
	{
		this.rewardGrid.Initialize(bonuses, true);
		SingletonAsset<SoundDatabase>.Instance.vipReward.Play();
		yield return this.box.Open();
		yield return FiberHelper.Wait(0.2f, (FiberHelper.WaitFlag)0);
		yield return this.rewardGrid.Animate(false, true);
		int daysLeft = VipManager.Instance.GetDaysLeft();
		this.daysDescriptionPart1.text = daysLeft.ToString();
		this.AnimCalenderWobble();
		yield return this.box.Close();
		UICamera.EnableInput();
		yield break;
	}

	private IEnumerator AnimStanchionAppear()
	{
		this.stanchion.SetActive(false);
		yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
		this.stanchion.SetActive(true);
		Vector3 targetScale = this.stanchion.transform.localScale;
		yield return FiberAnimation.ScaleTransform(this.stanchion.transform, Vector3.one * 0.75f, targetScale, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 1f);
		yield break;
	}

	private void AnimCalenderWobble()
	{
		Vector3 localScale = this.calender.transform.localScale;
		this.animFiber.Start(FiberAnimation.ScaleTransform(this.calender.transform, Vector3.one * 0.7f, localScale, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0f));
	}

	public UILabel waitTimer;

	public GameObject pivot_claim;

	public GameObject pivot_wait;

	public GameObject pivot_sync;

	public UILabel daysDescriptionPart1;

	public GameObject stanchion;

	public GameObject calender;

	public Instantiator dialog;

	public VipBox box;

	public RewardGrid rewardGrid;

	private bool needUpdateUI;

	private Vector3 boosterObjectPos;

	private Fiber logicFiber = new Fiber();

	private Fiber animFiber = new Fiber();
}
