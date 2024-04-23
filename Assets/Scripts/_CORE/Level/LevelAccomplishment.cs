using System;

public class LevelAccomplishment : ILevelAccomplishment
{
	[JsonSerializable("stars", null)]
	public int Stars { get; set; }

	[JsonSerializable("points", null)]
	public int Points { get; set; }
}
