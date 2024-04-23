using System;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads.RewardedVideo
{
	public interface IRewardedVideoRequirement
	{
		bool MeetsRequirement(AdGroupContext adGroupContext);
	}
}
