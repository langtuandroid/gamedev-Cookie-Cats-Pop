using System;
using Tactile;

[ConfigProvider("PushNotificationConfig")]
public class PushNotificationConfig
{
	[JsonSerializable("LevelRequiredForRegistration", null)]
	public int LevelRequiredForRegistration { get; set; }
}
