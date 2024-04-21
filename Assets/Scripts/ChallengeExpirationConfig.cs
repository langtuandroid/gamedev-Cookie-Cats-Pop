using System;

public class ChallengeExpirationConfig
{
	[JsonSerializable("Version", null)]
	public int Version { get; set; }

	[JsonSerializable("ExpirationDate", null)]
	public string ExpirationDate { get; set; }
}
