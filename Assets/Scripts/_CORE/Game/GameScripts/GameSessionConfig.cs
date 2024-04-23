using System;
using Tactile;

[ConfigProvider("GameSessionConfig")]
public class GameSessionConfig
{
	[JsonSerializable("PlayPauseForNewSession", null)]
	public int PlayPauseForNewSession { get; set; }
}
