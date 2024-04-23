using System;
using System.Collections;
using Fibers;
using NinjaUI;
using Tactile;
using UnityEngine;

public class BonusDropInfoView : UIView
{
	private DialogFrame Dialog
	{
		get
		{
			return this.dialog.GetInstance<DialogFrame>();
		}
	}

	protected override void ViewWillAppear()
	{
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.machine.gameObject.SetActive(true);
		this.coinPrize.gameObject.SetActive(false);
		this.prizePanel.SetActive(false);
		this.readyPivot.SetActive(Singleton<BonusDropManager>.Instance.PrizeAvailable);
		this.notReadyPivot.SetActive(!Singleton<BonusDropManager>.Instance.PrizeAvailable);
		if (Singleton<BonusDropManager>.Instance.PrizeAvailable)
		{
			this.Dialog.HasCloseButton = false;
		}
		else
		{
			this.collectLabel.text = string.Format(L.Get("{0} collected."), Singleton<BonusDropManager>.Instance.DropsCollected);
			this.infoLabel.text = string.Format(L.Get("Collect {0} Fortune Cookies to activate the Fortune Cat."), Singleton<BonusDropManager>.Instance.DropsRequiredForPrize);
		}
		this.bar.FillAmount = Mathf.Lerp(0.08f, 1f, Singleton<BonusDropManager>.Instance.NormalizedPrizeProgress);
	}

	private IEnumerator AnimateOpeningGift(InventoryItem boosterToGet)
	{
		SingletonAsset<SoundDatabase>.Instance.fortuneCatActivated.Play();
		foreach (GameObject gameObject in this.objectsToHideOnTap)
		{
			gameObject.SetActive(false);
		}
		Vector3 p = this.machine.localPosition;
		this.fortuneCat.state.SetAnimation(0, "spin cycle", true);
		yield return FiberAnimation.Animate(1f, delegate(float f)
		{
			this.machine.localPosition = p + Vector3.left * Mathf.Sin(Time.timeSinceLevelLoad * 20f * f) * 4f;
			this.machine.localScale = new Vector3(1f, 1f - f * 0.2f, 1f);
		});
		this.machine.gameObject.SetActive(false);
		this.prizePanel.gameObject.SetActive(true);
		this.Dialog.HasCloseButton = false;
		this.UpdatePrize(boosterToGet);
		yield return CameraShaker.ShakeDecreasing(0.3f, 10f, 30f, 0f, false);
		yield break;
	}

	private void UpdatePrize(InventoryItem boosterToGet)
	{
		foreach (GameObject gameObject in this.objectsToHideOnTap)
		{
			gameObject.SetActive(false);
		}
		if (boosterToGet != null)
		{
			this.boosterSprite.gameObject.SetActive(true);
			this.boosterName.gameObject.SetActive(true);
			BoosterMetaData metaData = InventoryManager.Instance.GetMetaData<BoosterMetaData>(boosterToGet);
			this.prizeName.text = L.Get("Free Booster!");
			this.boosterSprite.SpriteName = metaData.IconSpriteName;
			this.boosterName.text = metaData.Title;
		}
		else
		{
			this.boosterSprite.gameObject.SetActive(false);
			this.boosterName.gameObject.SetActive(false);
			this.prizeName.text = L.Get("Coins!");
			this.coinPrize.SetActive(true);
			GameObject gameObject2 = this.coinPrize;
			gameObject2.transform.parent = this.prizePivot.transform;
			gameObject2.SetLayerRecursively(base.gameObject.layer);
			gameObject2.transform.localPosition = Vector3.zero;
		}
	}

	private void UseMachine(UIEvent e)
	{
		ItemAmount itemAmount = Singleton<BonusDropManager>.Instance.ClaimPrize();
		if (itemAmount != null)
		{
			this.fiber.Start(this.AnimateOpeningGift(itemAmount.ItemId));
		}
		else
		{
			base.Close(0);
		}
	}

	private void CloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	public UIInstantiator dialog;

	public UILabel collectLabel;

	public UILabel infoLabel;

	public UIFillModifier bar;

	public GameObject readyPivot;

	public GameObject notReadyPivot;

	public Transform machine;

	public GameObject prizePivot;

	public GameObject coinPrize;

	public GameObject prizePanel;

	public UILabel prizeName;

	public UILabel boosterName;

	public UISprite boosterSprite;

	public SkeletonAnimation fortuneCat;

	public GameObject[] objectsToHideOnTap;

	private Fiber fiber = new Fiber();

	private Input input;
}
