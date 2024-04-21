using System;
using Tactile;

[ConfigProvider("ExtraAdConfig")]
public class ExtraAdConfig
{
	[JsonSerializable("CoinsForWatchingAd", null)]
	public int CoinsForWatchingAd { get; set; }
}
