using System;
using System.Collections.Generic;
using Tactile;

[ConfigProvider("ThemeHuntConfig")]
public class ThemeHuntConfig
{
	[JsonSerializable("LevelRequired", null)]
	public int LevelRequired { get; set; }

	[JsonSerializable("Hunts", typeof(ThemeHuntEntryConfig))]
	public List<ThemeHuntEntryConfig> Hunts { get; set; }

	public ThemeHuntEntryConfig GetHunt(string id)
	{
		return this.Hunts.Find((ThemeHuntEntryConfig o) => string.Equals(o.ID, id));
	}
}
