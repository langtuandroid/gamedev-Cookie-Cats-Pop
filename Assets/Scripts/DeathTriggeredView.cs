using System;
using UnityEngine;

public class DeathTriggeredView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		if (parameters.Length > 0)
		{
			this.session = (LevelSession)parameters[0];
		}
		for (int i = 0; i < this.statePivots.Length; i++)
		{
			this.statePivots[i].UpdateUI(this.session.SessionState);
		}
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ContinueAfterDeathWithBooster");
		ButtonPurchase instance = this.buyWithBoosterButton.GetInstance<ButtonPurchase>();
		instance.Title = L.Get("Play with shield");
		instance.Price = shopItem.CurrencyPrice;
		if (parameters.Length > 1)
		{
			this.scheduledBoosterButtonSwitcher.ShowScheduledBooster((bool)parameters[1]);
		}
	}

	private void BuyWithBoosterButton(UIEvent e)
	{
		if (base.IsClosing)
		{
			return;
		}
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ContinueAfterDeathWithBooster");
		ShopManager.Instance.GetShopItem("ContinueAfterDeathWithBooster");
		UserCareManager.Instance.StartCare("Coin");
		ShopManager.Instance.TrySpendCoins(shopItem, this.buyWithBoosterButton, delegate(bool succes)
		{
			UserCareManager.Instance.StopCare("Coin");
			if (succes)
			{
				base.Close(2);
			}
		});
	}

	private void DismissButton(UIEvent e)
	{
		base.Close(0);
	}

	protected override void ViewWillAppear()
	{
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
	}

	protected override void ViewGotFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = delegate()
		{
			UIViewManager.Instance.ShowView<ShopView>(new object[]
			{
				0
			});
		};
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
	}

	[SerializeField]
	private UIInstantiator buyWithBoosterButton;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private ScheduledBoosterButtonSwitcher scheduledBoosterButtonSwitcher;

	private LevelSession session;

	public DeathTriggeredView.SessionStatePivot[] statePivots;

	private CurrencyOverlay currencyOverlay;

	[Serializable]
	public class SessionStatePivot
	{
		public void UpdateUI(LevelSessionState state)
		{
			this.pivot.SetActive(this.state == state);
		}

		public LevelSessionState state;

		public GameObject pivot;
	}
}
