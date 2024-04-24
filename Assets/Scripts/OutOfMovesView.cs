using System;
using System.Collections;
using Fibers;
using TactileModules.Ads;
using TactileModules.Foundation;
using UnityEngine;

public class OutOfMovesView : UIView
{
	private GameObject Dialog
	{
		get
		{
			return this.dialog.GetInstance();
		}
	}

	private ButtonWithSpinner ButtonWithSpinner
	{
		get
		{
			return this.buttonWithSpinnerInstantiator.GetInstance<ButtonWithSpinner>();
		}
	}



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
		this.normalPivot.SetActive(true);
		this.noMatchesPivot.SetActive(false);
		this.description.text = this.GetDescriptionText();
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemContinue");
		ButtonPurchase instance = this.buyButton.GetInstance<ButtonPurchase>();
		instance.Title = L.Get("Keep Playing");
		instance.Price = shopItem.CurrencyPrice;
		if (parameters.Length > 1)
		{
			this.scheduledBoosterButtonSwitcher.ShowScheduledBooster((bool)parameters[1]);
		}
		
		this.ButtonWithSpinner.gameObject.SetActive(false);
	}

	private string GetDescriptionText()
	{
		if (this.session.Level.LevelAsset is EndlessLevel)
		{
			return L.Get("Get more bubbles and see how high you can get!");
		}
		int goalPiecesRemaining = this.session.GetGoalPiecesRemaining();
		if (goalPiecesRemaining == 1)
		{
			return L.Get("You only need to save 1 more kitten. Get more bubbles!");
		}
		return string.Format(L.Get("You only need to save {0} more kittens. Get more bubbles!"), goalPiecesRemaining);
	}

	private void BuyButton(UIEvent e)
	{
		if (base.IsClosing)
		{
			return;
		}
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemContinue");
		UserCareManager.Instance.StartCare("Coin");
		ShopManager.Instance.TrySpendCoins(shopItem, this.buyButton, delegate(bool succes)
		{
			UserCareManager.Instance.StopCare("Coin");
			if (succes)
			{
				base.Close(1);
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

	private void OnDisable()
	{
		this.requestingVideoFiber.Terminate();
	}

	[SerializeField]
	private UIInstantiator buyButton;

	[SerializeField]
	private UILabel description;

	[SerializeField]
	private GameObject normalPivot;

	[SerializeField]
	private GameObject noMatchesPivot;

	[SerializeField]
	private ScheduledBoosterButtonSwitcher scheduledBoosterButtonSwitcher;

	[SerializeField]
	private Instantiator dialog;

	[SerializeField]
	private UIInstantiator buttonWithSpinnerInstantiator;

	private Fiber requestingVideoFiber = new Fiber();

	private string rewardedVideoPlacement = "OutOfMoves";

	private LevelSession session;

	public OutOfMovesView.SessionStatePivot[] statePivots;

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
