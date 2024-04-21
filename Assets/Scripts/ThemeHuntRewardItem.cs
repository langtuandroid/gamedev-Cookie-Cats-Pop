using System;
using System.Collections.Generic;

public class ThemeHuntRewardItem
{
	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }

	[JsonSerializable("RewardType", null)]
	public string RewardType { get; set; }

	[JsonSerializable("ThemeItemsRequired", null)]
	public int ThemeItemsRequired { get; set; }
}
