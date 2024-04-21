using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("SeagullConfig")]
public class SeagullConfig
{
	[JsonSerializable("IsActive", null)]
	public bool IsActive { get; set; }

	[JsonSerializable("SpawnIntervalInLevelEnds", null)]
	public int SpawnIntervalInLevelEnds { get; set; }

	[JsonSerializable("Rewards", typeof(ItemAmount))]
	public List<ItemAmount> Rewards { get; set; }
}
