using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("ShopConfig")]
public class ShopConfig
{
	public ShopConfig()
	{
		this.ShopItems = new List<ShopItem>();
	}

	[JsonSerializable("ShopItems", typeof(ShopItem))]
	public List<ShopItem> ShopItems { get; set; }

	[JsonSerializable("ShowInfiniteLifeRibbon", null)]
	public bool ShowInfiniteLifeRibbon { get; set; }

	[JsonSerializable("ShowMostPopularSticker", null)]
	public bool ShowMostPopularSticker { get; set; }

	[JsonSerializable("ShowBestOfferSticker", null)]
	public bool ShowBestOfferSticker { get; set; }

	[JsonSerializable("ShowOffersInShop", null)]
	public bool ShowOffersInShop { get; set; }

	[JsonSerializable("ShowSaveAmountText", null)]
	public bool ShowSaveAmountText { get; set; }

	[JsonSerializable("RewardForWatchingAd", typeof(ItemAmount))]
	public List<ItemAmount> RewardForWatchingAd { get; set; }

	[JsonSerializable("MinimumVersionToShowAdRemovalTextIOS", null)]
	public string MinimumVersionToShowAdRemovalTextIOS { get; set; }

	public bool ShouldShowRemoveAdsText
	{
		get
		{
			if (this.MinimumVersionToShowAdRemovalTextIOS != null)
			{
				string[] array = this.MinimumVersionToShowAdRemovalTextIOS.Split(new char[]
				{
					'.'
				});
				string[] array2 = SystemInfoHelper.BundleShortVersion.Split(new char[]
				{
					'.'
				});
				if (array.Length == 3 && array2.Length == 3)
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					int num6 = 0;
					int.TryParse(array[0], out num);
					int.TryParse(array[1], out num2);
					int.TryParse(array[2], out num3);
					int.TryParse(array2[0], out num4);
					int.TryParse(array2[1], out num5);
					int.TryParse(array2[2], out num6);
					return num4 <= num && num5 <= num2 && num6 <= num3;
				}
			}
			return false;
		}
	}
}
