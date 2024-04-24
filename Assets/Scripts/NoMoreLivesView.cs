using System;
using Fibers;
using Tactile;
using UnityEngine;

public class NoMoreLivesView : UIView
{
	
	private void OnDestroy()
	{
		this.watchFiber.Terminate();
	}

	protected override void ViewLoad(object[] parameters)
	{
		this.rewardItem =  "Life";
	}

	protected override void ViewWillAppear()
	{
		this.currencyOverlay = base.ObtainOverlay<CurrencyOverlay>();
		this.livesOverlay = base.ObtainOverlay<LivesOverlay>();
	}

	protected override void ViewDidDisappear()
	{
		base.ReleaseOverlay<CurrencyOverlay>();
		if (this.livesOverlay != null)
		{
			base.ReleaseOverlay<LivesOverlay>();
		}
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
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = delegate()
			{
				base.Close(0);
			};
		}
		this.buyPriceLabel.text = ShopManager.Instance.GetShopItem("ShopItemExtraLives").CurrencyPrice.ToString();
	}

	protected override void ViewLostFocus()
	{
		this.currencyOverlay.OnCoinButtonClicked = null;
		if (this.livesOverlay != null)
		{
			this.livesOverlay.OnLifeButtonClicked = null;
		}
	}
	

	private void Update()
	{
		int num = LivesManager.Instance.GetRegenerationTimeLeft();
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)num);
		this.timerLabel.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
	}
	

	private void ButtonCloseClicked(UIEvent e)
	{
		base.Close(0);
	}
	

	private void BuyLives(UIEvent e)
	{
		ShopItem shopItem = ShopManager.Instance.GetShopItem("ShopItemExtraLives");
		int tmpLives = InventoryManager.Instance.Lives;
		Vector3 purchasePos = this.buyPriceButtonPivot.transform.position + Vector3.back * 10f;
		ShopManager.Instance.TrySpendCoins(shopItem, this.buyPriceButtonPivot, delegate(bool succes)
		{
			if (succes)
			{
				UICamera.DisableInput();
				if (this.livesOverlay != null)
				{
					int visibleLives = InventoryManager.Instance.Lives - tmpLives;
					this.livesOverlay.GiveLife(purchasePos, visibleLives, 1f, delegate
					{
						this.livesOverlay.Refresh();
						UICamera.EnableInput();
						this.Close(1);
					});
				}
				else
				{
					UICamera.EnableInput();
					this.Close(1);
				}
			}
		});
	}
	

	[SerializeField]
	private UILabel timerLabel;

	[SerializeField]
	private GameObject buyPriceButtonPivot;

	[SerializeField]
	private UILabel buyPriceLabel;

	[SerializeField]
	private UISprite buyButtonHeart;

	private LivesOverlay livesOverlay;

	private CurrencyOverlay currencyOverlay;

	private string rewardItem;

	private Fiber watchFiber = new Fiber();

	private string rewardedVideoPlacement = "OutOfLives";
}
