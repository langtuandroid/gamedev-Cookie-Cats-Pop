using System;

public class LocalScore
{
	[JsonSerializable("l", null)]
	public int Leaderboard { get; set; }

	[JsonSerializable("s", null)]
	public int Score { get; set; }

	[JsonSerializable("v", null)]
	public int VideoId { get; set; }

	[JsonSerializable("sc", null)]
	public bool SubmittedToCloud { get; set; }
}
