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

	private InAppPurchaseManager InAppPurchaseManager
	{
		get
		{
			return ManagerRepository.Get<InAppPurchaseManager>();
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
		this.buyButton.GetInstance<ButtonWithTitle>().Title = shopItem.FormattedPricePreferIAP(this.InAppPurchaseManager);
	}

	private void ButtonCloseClicked(UIEvent e)
	{
		this.DismissView("close");
	}

	private void ButtonBuyClicked(UIEvent e)
	{
		FiberCtrl.Pool.Run(this.Purchase(), false);
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

	private IEnumerator Purchase()
	{
		if (this.processingPurchase)
		{
			yield break;
		}
		this.processingPurchase = true;
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemVip");
		InAppProduct product = this.InAppPurchaseManager.GetProductForIdentifier(shopItem.FullIAPIdentifier);
		bool success = false;
		if (product != null)
		{
			yield return this.InAppPurchaseManager.DoInAppPurchase(product, delegate(string purchaseSessionId, string transactionId, InAppPurchaseStatus resultStatus)
			{
				success = (resultStatus == InAppPurchaseStatus.Succeeded);
			});
		}
		this.processingPurchase = false;
		if (success)
		{
			SingletonAsset<UISetup>.Instance.purchaseSuccessful.Play();
			UICamera.DisableInput();
			UIViewManager.UIViewStateGeneric<PurchaseAcknowledgementView> acknowledgementvs = UIViewManager.Instance.ShowView<PurchaseAcknowledgementView>(new object[0]);
			yield return acknowledgementvs.WaitForClose();
			UICamera.EnableInput();
			this.DismissView("buy");
		}
		yield break;
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
