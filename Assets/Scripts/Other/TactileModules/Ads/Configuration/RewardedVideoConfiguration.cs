using System;
using System.Collections.Generic;
using ConfigSchema;

namespace TactileModules.Ads.Configuration
{
	[RequireAll]
	public class RewardedVideoConfiguration
	{
		public RewardedVideoConfiguration()
		{
			this.RewardedVideoContextConfiguration = new List<RewardedVideoContextConfiguration>();
		}

		[JsonSerializable("RewardedVideoContextConfiguration", typeof(RewardedVideoContextConfiguration))]
		public List<RewardedVideoContextConfiguration> RewardedVideoContextConfiguration { get; set; }
	}
}
