using System;
using Tactile;

[ConfigProvider("FacebookNotification")]
public class FacebookNotification
{
	[JsonSerializable("LevelRequiredForPlayWithFriendsView", null)]
	public int LevelRequiredForPlayWithFriendsView { get; set; }
}
