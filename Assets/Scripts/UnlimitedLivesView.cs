using System;
using Tactile;
using UnityEngine;

public class UnlimitedLivesView : UIView
{
	protected override void ViewLoad(object[] parameters)
	{
		int hours = TimeSpan.FromSeconds((double)ConfigurationManager.Get<LivesConfig>().InfiniteLivesDurationInSeconds).Hours;
		this.descriptionLabel.text = string.Format(L.Get("Buy {0} or more coins and receive {1} hours of Unlimited Lives!"), this.MinCoinsForUnlimitedLives(), hours);
	}

	private void CloseClicked(UIEvent e)
	{
		base.Close(0);
	}

	private void ContinueClicked(UIEvent e)
	{
		UIViewManager.Instance.ShowView<ShopView>(new object[]
		{
			0
		});
		base.Close(0);
	}

	private int MinCoinsForUnlimitedLives()
	{
		int num = int.MaxValue;
		foreach (ShopItem shopItem in ShopManager.Instance.ShopItemsWithIAPForItems("Coin"))
		{
			if (shopItem.CustomTag.Contains("unlimitedLives") && shopItem.CoinAmount < num)
			{
				num = shopItem.CoinAmount;
			}
		}
		return num;
	}

	[SerializeField]
	private UILabel descriptionLabel;
}
