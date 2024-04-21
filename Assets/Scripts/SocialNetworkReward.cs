using System;
using System.Collections.Generic;

public class SocialNetworkReward
{
	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
