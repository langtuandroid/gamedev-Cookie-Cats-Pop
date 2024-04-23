using System;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads.Configuration
{
	public interface IAdConfigurationProvider
	{
		AdConfiguration ActiveConfiguration { get; }

		RewardedVideoContextConfiguration GetRewardedVideoPlacementConfig(AdGroupContext adGroupContext);
	}
}
