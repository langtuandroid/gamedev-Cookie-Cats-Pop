using System;
using System.Collections;
using Tactile;
using TactileModules.Foundation;
using UnityEngine;

public class VipProgramNotSubscriberView : UIView
{
	private ConfigurationManager ConfigurationManager
	{
		get
		{
			return ManagerRepository.Get<ConfigurationManager>();
		}
	}
	
	protected override void ViewLoad(object[] parameters)
	{
		this.pivot_locked.SetActive(!VipManager.Instance.IsVipProgramUnlocked());
		this.pivot_unlocked.SetActive(VipManager.Instance.IsVipProgramUnlocked());
		int num = 30;
		this.bonusDescriptionLocked.text = string.Format(this.bonusDescriptionLocked.text, num.ToString());
		this.bonusDescriptionUnlocked.text = string.Format(this.bonusDescriptionUnlocked.text, num.ToString());
		this.daysDescriptionPart1.text = num.ToString();
		if (VipManager.Instance.IsVipProgramUnlocked())
		{
			this.initializeReadyView();
		}
		else
		{
			this.initializeNotReadyView();
		}
	}

	private void initializeNotReadyView()
	{
		this.lockedDescriptionPart2.text = string.Format(this.lockedDescriptionPart2.text, this.ConfigurationManager.GetConfig<VipProgramConfig>().LevelRequiredForVip);
	}

	private void initializeReadyView()
	{
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemVip");
		//this.buyButton.GetInstance<ButtonWithTitle>().Title = shopItem.FormattedPricePreferIAP(this.InAppPurchaseManager);
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		this.DismissView("close");
	}
	
	private void DismissView(string button)
	{
		if (this.processingPurchase)
		{
			return;
		}
		base.Close(0);
		if (button == "buy")
		{
			UIViewManager.Instance.ShowView<VipProgramSubscriberView>(new object[0]);
		}
	}
	

	public GameObject pivot_locked;

	public GameObject pivot_unlocked;

	public UILabel lockedDescriptionPart2;

	public UIInstantiator buyButton;

	public UILabel bonusDescriptionLocked;

	public UILabel bonusDescriptionUnlocked;

	public UILabel daysDescriptionPart1;

	private bool processingPurchase;
}
