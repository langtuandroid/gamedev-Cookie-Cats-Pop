using System;
using System.Collections.Generic;

public class ShopItem
{
	public ShopItem()
	{
		this.Rewards = new List<ItemAmount>();
	}

	[JsonSerializable("Identifier", null)]
	public string Type { get; set; }

	[JsonSerializable("IAPIdentifier", null)]
	public string IAPIdentifier { private get; set; }

	[JsonSerializable("CurrencyPrice", null)]
	public int CurrencyPrice { get; set; }

	[JsonSerializable("Tag", null)]
	public string CustomTag { get; set; }

	[JsonSerializable("InventoryItems", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }

	public string PartialIAPIdentifier
	{
		get
		{
			return this.IAPIdentifier;
		}
	}

	public string FullIAPIdentifier
	{
		get
		{
			if (!string.IsNullOrEmpty(this.IAPIdentifier))
			{
				return InAppProductTactileInfo.IdentifierPrefix + this.IAPIdentifier;
			}
			return this.IAPIdentifier;
		}
	}

	public InAppProduct GetInAppProduct(InAppPurchaseManagerBase inAppPurchaseManager)
	{
		return inAppPurchaseManager.GetProductForIdentifier(this.FullIAPIdentifier);
	}

	public string FormattedPricePreferIAP(InAppPurchaseManagerBase inAppPurchaseManager)
	{
		InAppProduct inAppProduct = this.GetInAppProduct(inAppPurchaseManager);
		return (inAppProduct != null) ? inAppProduct.FormattedPrice : "-";
	}

	public int Amount(string inventoryItemId)
	{
		foreach (ItemAmount itemAmount in this.Rewards)
		{
			if (itemAmount.ItemId == inventoryItemId)
			{
				return itemAmount.Amount;
			}
		}
		return 0;
	}

	public int CoinAmount
	{
		get
		{
			return this.Amount("Coin");
		}
	}

	public float CalculatePricePerItem(InAppPurchaseManagerBase inAppPurchaseManager)
	{
		int num = 0;
		foreach (ItemAmount itemAmount in this.Rewards)
		{
			num += itemAmount.Amount;
		}
		if (num == 0)
		{
			num = 1;
		}
		InAppProduct inAppProduct = this.GetInAppProduct(inAppPurchaseManager);
		if (inAppProduct != null)
		{
			return (float)inAppProduct.TactileInfo.Price / (float)num;
		}
		return (float)this.CurrencyPrice / (float)num;
	}

	public override string ToString()
	{
		return string.Format("[ShopItem: Type={0}, IAPIdentifier={1}, CurrencyPrice={2}, CustomTag={3}, Rewards={4}, PartialIAPIdentifier={5}, FullIAPIdentifier={6}, CoinAmount={7}]", new object[]
		{
			this.Type,
			this.IAPIdentifier,
			this.CurrencyPrice,
			this.CustomTag,
			this.Rewards,
			this.PartialIAPIdentifier,
			this.FullIAPIdentifier,
			this.CoinAmount
		});
	}
}
