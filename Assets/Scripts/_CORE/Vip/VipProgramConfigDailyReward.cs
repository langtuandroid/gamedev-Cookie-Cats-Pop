using System;
using System.Collections.Generic;

public class VipProgramConfigDailyReward
{
	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
