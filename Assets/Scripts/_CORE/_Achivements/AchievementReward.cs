using System;
using System.Collections.Generic;

public class AchievementReward
{
	[JsonSerializable("ID", null)]
	public string ID { get; set; }

	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
