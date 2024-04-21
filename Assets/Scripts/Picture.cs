using System;

public class Picture
{
	[JsonSerializable("data", null)]
	public PictureData Data { get; set; }
}
