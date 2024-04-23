using System;

public class GateMetaData : LevelMetaData
{
	[JsonSerializable("gateIndex", null)]
	public int gateIndex { get; set; }

	[JsonSerializable("levelCount", null)]
	public int levelCount { get; set; }
}
