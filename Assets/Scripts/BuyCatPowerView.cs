using System;

public class BuyCatPowerView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.item = (ShopItem)parameters[0];
		this.isTutorial = (parameters.Length > 1 && (bool)parameters[1]);
		CPShopItemMetaData cpshopItemMetaData = ShopManager.Instance.GetMetaDataForShopItem(this.item) as CPShopItemMetaData;
		this.description.text = L.Get(cpshopItemMetaData.Description);
		this.title.text = L.Get(cpshopItemMetaData.Title);
		this.image.TextureResource = "Graphics/PowerCats/ui_feed_" + this.item.Type;
		this.image.CorrectAspect(AspectCorrection.Fit);
		ButtonPurchase instance = this.buyButton.GetInstance<ButtonPurchase>();
		if (this.isTutorial)
		{
			instance.Title = L.Get("Free");
			instance.Price = 0;
		}
		else
		{
			instance.Title = L.Get(cpshopItemMetaData.BuyButtonText);
			instance.Price = this.item.CurrencyPrice;
		}
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

	private void BuyButtonPressed(UIEvent e)
	{
		if (base.ClosingResult != null)
		{
			return;
		}
		if (this.isTutorial)
		{
			GameEventManager.Instance.Emit(51);
			base.Close(BuyCatPowerView.Result.SuccessfullyBought);
		}
		else
		{
			ShopManager.Instance.TrySpendCoins(this.item, this.buyButton, delegate(bool succes)
			{
				if (succes)
				{
					base.Close(BuyCatPowerView.Result.SuccessfullyBought);
				}
			});
		}
	}

	private void CloseButtonPressed(UIEvent e)
	{
		base.Close(BuyCatPowerView.Result.FailedToBuy);
	}

	public UIInstantiator buyButton;

	public UILabel description;

	public UIInstantiator dialog;

	public UILabel title;

	public UIBytesQuad image;

	private ShopItem item;

	private bool isTutorial;

	private CurrencyOverlay currencyOverlay;

	public enum Result
	{
		SuccessfullyBought,
		FailedToBuy
	}
}
