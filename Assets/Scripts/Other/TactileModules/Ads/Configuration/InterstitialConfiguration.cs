using System;
using ConfigSchema;

namespace TactileModules.Ads.Configuration
{
	[RequireAll]
	public class InterstitialConfiguration
	{
		[Description("The level requirement for seeing interstitials of any type")]
		[JsonSerializable("LevelRequiredForInterstitials", null)]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		public int LevelRequiredForInterstitials { get; set; }

		[Description("The cooldown in seconds for interstitials")]
		[JsonSerializable("MinimumSecondsBetweenInterstitials", null)]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		public int MinimumSecondsBetweenInterstitials { get; set; }

		[Description("The amount of seconds that needs to have passed before interstitials can start showing.")]
		[JsonSerializable("MinimumSecondsBeforeFirstAd", null)]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		public int MinimumSecondsBeforeFirstAd { get; set; }
	}
}
