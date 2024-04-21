using System;
using Tactile;

public class BuyShopItemView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		this.item = (ShopItem)parameters[0];
		this.isTutorial = (parameters.Length > 1 && (bool)parameters[1]);
		CPShopItemMetaData cpshopItemMetaData = ShopManager.Instance.GetMetaDataForShopItem(this.item) as CPShopItemMetaData;
		this.description.text = L.Get(cpshopItemMetaData.Description);
		this.title.text = L.Get(cpshopItemMetaData.Title);
		this.icon.SpriteName = cpshopItemMetaData.ImageSpriteName;
		string text = string.Empty;
		if (this.item.Rewards.Count == 1)
		{
			int amount = this.item.Rewards[0].Amount;
			if (amount > 1)
			{
				InventoryItemMetaData metaData = InventoryManager.Instance.GetMetaData(this.item.Rewards[0].ItemId);
				text = metaData.FormattedQuantity(amount);
			}
		}
		this.quantityLabel.text = text;
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
			base.Close(BuyShopItemView.Result.SuccessfullyBought);
		}
		else
		{
			ShopManager.Instance.TrySpendCoins(this.item, this.buyButton, delegate(bool succes)
			{
				if (succes)
				{
					base.Close(BuyShopItemView.Result.SuccessfullyBought);
				}
			});
		}
	}

	private void CloseButtonPressed(UIEvent e)
	{
		base.Close(BuyShopItemView.Result.FailedToBuy);
	}

	public UIInstantiator buyButton;

	public UILabel description;

	public UISprite icon;

	public UIInstantiator dialog;

	public UILabel title;

	public UILabel quantityLabel;

	private ShopItem item;

	private bool isTutorial;

	private CurrencyOverlay currencyOverlay;

	public enum Result
	{
		SuccessfullyBought,
		FailedToBuy
	}
}
