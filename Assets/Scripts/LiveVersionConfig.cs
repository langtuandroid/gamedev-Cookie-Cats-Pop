using System;
using Tactile;

[ConfigProvider("LiveVersionConfig")]
public class LiveVersionConfig
{
	[JsonSerializable("VersionNumber", null)]
	public int VersionNumber { get; set; }
}
