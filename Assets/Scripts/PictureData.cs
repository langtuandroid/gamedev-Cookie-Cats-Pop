using System;

public class PictureData
{
	[JsonSerializable("height", null)]
	public int Height { get; set; }

	[JsonSerializable("width", null)]
	public int Width { get; set; }

	[JsonSerializable("is_silhouette", null)]
	public bool IsSilhouette { get; set; }

	[JsonSerializable("url", null)]
	public string URL { get; set; }
}
