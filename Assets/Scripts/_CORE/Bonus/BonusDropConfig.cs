using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("BonusDropConfig")]
public class BonusDropConfig
{
	[JsonSerializable("PresentsSpawnIntervalInMoves", null)]
	public int SpawnIntervalInMoves { get; set; }

	[JsonSerializable("PresentsRequiredForBonus", null)]
	public int ItemsRequiredForPrize { get; set; }

	[JsonSerializable("LevelRequiredForPresents", null)]
	public int LevelRequired { get; set; }

	[JsonSerializable("RewardPool", typeof(ItemAmount))]
	public List<ItemAmount> RewardPool { get; set; }
}
