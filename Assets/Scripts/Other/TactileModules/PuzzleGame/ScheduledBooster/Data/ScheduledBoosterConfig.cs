using System;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.ScheduledBooster.Data
{
	[ConfigProvider("LimitedAvailabilityBoosterConfig")]
	public class ScheduledBoosterConfig
	{
		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
