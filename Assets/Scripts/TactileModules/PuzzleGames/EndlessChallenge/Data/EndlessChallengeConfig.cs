using System;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.EndlessChallenge.Data
{
	[ObsoleteJsonName("MaxRequiredLevelsLeft")]
	[ConfigProvider("EndlessChallengeConfig")]
	public class EndlessChallengeConfig
	{
		[JsonSerializable("IsActive", null)]
		public bool IsActive { get; set; }

		[JsonSerializable("LevelRequiredForEndlessChallenge", null)]
		public int LevelRequiredForEndlessChallenge { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
