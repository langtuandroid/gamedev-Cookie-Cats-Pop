using System;
using ConfigSchema;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads.Configuration
{
	public class RewardedVideoContextConfiguration
	{
		[Description("A context describes a location/place in the game where an ad related event can happen.")]
		[StringEnum(typeof(AdGroupContext), "GetIdentifiers")]
		[JsonSerializable("RewardedVideoContext", null)]
		public string RewardedVideoContext { get; set; }

		[Description("Kill-switch for the rewarded video in this context")]
		[JsonSerializable("IsActive", null)]
		public bool IsActive { get; set; }

		[Description("The number of seconds between each rewarded video that can be played in this context")]
		[JsonSerializable("CoolDownInSeconds", null)]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		public int CoolDownInSeconds { get; set; }

		[Description("The level requirement for being able to watch rewarded videos in this context")]
		[JsonSerializable("LevelRequired", null)]
		[DevelopmentState(new DevelopmentStateAttribute.DevelopmentState[]
		{
			DevelopmentStateAttribute.DevelopmentState.dev,
			DevelopmentStateAttribute.DevelopmentState.prod
		})]
		public int LevelRequired { get; set; }
	}
}
