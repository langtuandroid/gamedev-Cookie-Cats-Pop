using System;
using System.Collections.Generic;

public class ThemeHuntEntryConfig
{
	[JsonSerializable("ID", null)]
	public string ID { get; set; }

	[JsonSerializable("SpawnFrequency", null)]
	public int SpawnFrequency { get; set; }

	[JsonSerializable("MaxSpawnItems", null)]
	public int MaxSpawnItems { get; set; }

	[JsonSerializable("Rewards", typeof(ThemeHuntRewardItem))]
	public List<ThemeHuntRewardItem> Rewards { get; set; }
}
