using System;
using ConfigSchema;
using Tactile;

[ConfigProvider("DifficultyConfig")]
[RequireAll]
public class DifficultyConfig
{
	[JsonSerializable("ShowLevelDifficulty", null)]
	public bool ShowLevelDifficulty { get; set; }
}
