using System;
using Tactile;

[ConfigProvider("ExperienceConfig")]
public class ExperienceConfig
{
	public ExperienceConfig()
	{
		this.MediumSpeedAtLevel = 15;
		this.HighSpeedAtLevel = 100;
	}

	[JsonSerializable("MediumSpeedAtLevel", null)]
	public int MediumSpeedAtLevel { get; set; }

	[JsonSerializable("HighSpeedAtLevel", null)]
	public int HighSpeedAtLevel { get; set; }

	[JsonSerializable("FixedFirstFreebies", null)]
	public bool FixedFirstFreebies { get; set; }
}
