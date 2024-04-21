using System;
using System.Collections.Generic;
using ConfigSchema;
using Tactile;

namespace TactileModules.Ads.Configuration
{
	[ObsoleteJsonName(new string[]
	{
		"LevelRequiredForAds",
		"MinSecondsBeforeFirstAd",
		"MinSecondsBetweenInterstitials",
		"LevelRequiredForStartSession",
		"LevelRequiredForSceneTransitions",
		"WatchVideoGetLifeRegenerationSeconds",
		"LevelRequiredForFreeBoosterVideo",
		"VideoEveryNthSceneTransitions",
		"NoMoreAdsMessageShowMilliseconds"
	})]
	[ConfigProvider("AdConfig")]
	public class AdConfigContainer
	{
		[JsonSerializable("Default", null)]
		public AdConfiguration Default { get; set; }

		[Platform(new PlatformAttribute.Platform[]
		{
			PlatformAttribute.Platform.android,
			PlatformAttribute.Platform.ios,
			PlatformAttribute.Platform.fireos,
			PlatformAttribute.Platform.Default
		})]
		[JsonSerializable("Custom", typeof(AdConfiguration))]
		public List<AdConfiguration> Custom { get; set; }
	}
}
