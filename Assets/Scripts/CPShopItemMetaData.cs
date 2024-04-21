using System;
using UnityEngine;

public class CPShopItemMetaData : ShopItemMetaData
{
	public string BuyButtonText
	{
		get
		{
			if (string.IsNullOrEmpty(this.buyButtonText))
			{
				return "Buy";
			}
			return this.buyButtonText;
		}
	}

	[SerializeField]
	private string buyButtonText = "Buy";
}
