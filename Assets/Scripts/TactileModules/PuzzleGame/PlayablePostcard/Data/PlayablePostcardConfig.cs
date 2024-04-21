using System;
using Tactile;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGame.PlayablePostcard.Data
{
	[ConfigProvider("PlayablePostcardConfig")]
	public class PlayablePostcardConfig
	{
		[JsonSerializable("LevelRequired", null)]
		public int LevelRequired { get; set; }

		[JsonSerializable("FeatureNotificationSettings", null)]
		public FeatureNotificationSettings FeatureNotificationSettings { get; set; }
	}
}
