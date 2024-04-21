using System;
using UnityEngine;

public class ButtonPurchase : ButtonWithIcon
{
	[Instantiator.SerializeLocalizableProperty]
	public int Price
	{
		get
		{
			return this.price;
		}
		set
		{
			this.price = value;
			this.ReflectState();
		}
	}

	[Instantiator.SerializeLocalizableProperty]
	public string Title
	{
		get
		{
			return this.title;
		}
		set
		{
			this.title = value;
			this.ReflectState();
		}
	}

	public void SetByShopItemIdentifier(ShopItemIdentifier id, Action purchaseSuccessHandler)
	{
		this.shopItemIdentifier = id;
		this.purchaseSuccessHandler = purchaseSuccessHandler;
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			this.Title = L.Get(this.Title);
		}
	}

	protected override void HandleButtonClicked(UIButton b)
	{
		base.HandleButtonClicked(b);
		if (this.purchaseSuccessHandler != null)
		{
			ShopManager.Instance.TrySpendCoins(this.shopItemIdentifier, this, delegate(bool succes)
			{
				if (succes)
				{
					this.purchaseSuccessHandler();
				}
			});
		}
	}

	protected override void ReflectState()
	{
		base.ReflectState();
		if (!string.IsNullOrEmpty(this.shopItemIdentifier))
		{
			CPShopItemMetaData metaData = ShopManager.Instance.GetMetaData<CPShopItemMetaData>(this.shopItemIdentifier);
			if (metaData != null)
			{
				ShopItem shopItem = ShopManager.Instance.GetShopItem(this.shopItemIdentifier);
				if (shopItem != null)
				{
					this.title = metaData.BuyButtonText;
					this.price = shopItem.CurrencyPrice;
				}
			}
		}
		if (this.price > 0)
		{
			this.label.text = string.Format(string.Format("{0}   {1}[C]", this.title, this.price), new object[0]);
		}
		else
		{
			this.label.text = this.title;
		}
	}

	[SerializeField]
	private UILabel label;

	private int price = 70;

	private string title;

	private ShopItemIdentifier shopItemIdentifier;

	private Action purchaseSuccessHandler;
}
