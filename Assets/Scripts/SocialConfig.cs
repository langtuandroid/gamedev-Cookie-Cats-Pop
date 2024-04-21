using System;
using Tactile;

[ConfigProvider("SocialConfig")]
public class SocialConfig
{
	[JsonSerializable("TwitterTrackUrl", null)]
	public string TwitterTrackUrl { get; set; }

	[JsonSerializable("GooglePlusTrackUrl", null)]
	public string GooglePlusTrackUrl { get; set; }

	[JsonSerializable("FacebookAppUrl", null)]
	public string FacebookAppUrl { get; set; }

	[JsonSerializable("FacebookAppUrlAndroid", null)]
	public string FacebookAppUrlAndroid { get; set; }

	[JsonSerializable("InstagramAppUrl", null)]
	public string InstagramAppUrl { get; set; }

	[JsonSerializable("YouTubeAppUrl", null)]
	public string YouTubeAppUrl { get; set; }

	[JsonSerializable("FacebookNormalUrl", null)]
	public string FacebookNormalUrl { get; set; }

	[JsonSerializable("TwitterAppUrl", null)]
	public string TwitterAppUrl { get; set; }

	[JsonSerializable("TwitterNormalUrl", null)]
	public string TwitterNormalUrl { get; set; }

	[JsonSerializable("InstagramNormalUrl", null)]
	public string InstagramNormalUrl { get; set; }

	[JsonSerializable("YouTubeNormalUrl", null)]
	public string YouTubeNormalUrl { get; set; }

	[JsonSerializable("LevelRequiredForLoginToFacebookView", null)]
	public int LevelRequiredForLoginToFacebookView { get; set; }

	[JsonSerializable("LevelRequiredForSharingLevelCompleted", null)]
	public int LevelRequiredForSharingLevelCompleted { get; set; }

	[JsonSerializable("MinimumStarsRequiredToShowSubsequentShareBee", null)]
	public int MinimumStarsRequiredToShowSubsequentShareBee { get; set; }

	[JsonSerializable("FacebookRewards", null)]
	public SocialNetworkReward FacebookRewards { get; set; }

	[JsonSerializable("TwitterRewards", null)]
	public SocialNetworkReward TwitterRewards { get; set; }

	[JsonSerializable("InstagramRewards", null)]
	public SocialNetworkReward InstagramRewards { get; set; }

	[JsonSerializable("YouTubeRewards", null)]
	public SocialNetworkReward YouTubeRewards { get; set; }

	[JsonSerializable("RequiredLevel", null)]
	public int RequiredLevel { get; set; }
}
