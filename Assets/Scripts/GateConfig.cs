using System;
using Tactile;

[ConfigProvider("GateConfig")]
public class GateConfig
{
	[JsonSerializable("QuestRegenerationTime", null)]
	public int QuestRegenerationTime { get; set; }
}
