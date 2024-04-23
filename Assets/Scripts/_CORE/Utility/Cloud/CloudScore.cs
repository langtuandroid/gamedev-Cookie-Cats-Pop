using System;

public class CloudScore
{
	[JsonSerializable("leaderboard", null)]
	public int Leaderboard { get; set; }

	[JsonSerializable("score", null)]
	public int Score { get; set; }

	[JsonSerializable("videoId", null)]
	public int VideoId { get; set; }

	[JsonSerializable("userId", null)]
	public string UserId { get; set; }

	public string facebookId;

	public string displayName;

	public string deviceId = string.Empty;
}
