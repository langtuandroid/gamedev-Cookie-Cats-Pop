using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.Ads.Configuration
{
	[ObsoleteJsonName(new string[]
	{
		"LevelRequiredForFreeBoosterVideo",
		"WatchVideoGetLifeRegenerationSeconds, RewardVideoConfig"
	})]
	public class AdConfiguration
	{
		public AdConfiguration()
		{
			this.Countries = new CountryCodeContainer();
			this.RewardedVideoConfiguration = new RewardedVideoConfiguration();
			this.InterstitialConfiguration = new InterstitialConfiguration();
		}

		[JsonSerializable("ID", null)]
		public string ID { get; set; }

		[JsonSerializable("RewardedVideoConfiguration", null)]
		[Required]
		public RewardedVideoConfiguration RewardedVideoConfiguration { get; set; }

		[JsonSerializable("InterstitialConfiguration", null)]
		[Required]
		public InterstitialConfiguration InterstitialConfiguration { get; set; }

		[JsonSerializable("AquisitionChannels", typeof(AnalyticsBase.AdjustAquisitionChannel))]
		public List<AnalyticsBase.AdjustAquisitionChannel> AquisitionChannels { get; set; }

		[JsonSerializable("Countries", null)]
		public CountryCodeContainer Countries { get; set; }
	}
}
