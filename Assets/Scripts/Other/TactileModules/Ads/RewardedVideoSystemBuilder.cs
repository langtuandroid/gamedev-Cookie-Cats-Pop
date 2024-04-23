using System;
using TactileModules.Ads.Configuration;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Ads
{
	public static class RewardedVideoSystemBuilder
	{
		public static RewardedVideoPresenter Build(IMusicControlProvider musicControlProvider, IAdConfigurationProvider adConfigurationProvider, IAnalytics tactileAnalytics)
		{
			RewardedVideoCoolDowns rewardedVideoCoolDowns = new RewardedVideoCoolDowns(adConfigurationProvider, new RewardedVideoStoreFactory());
			RewardedVideoAnalytics rewardedVideoAnalytics = new RewardedVideoAnalytics(tactileAnalytics);
			return new RewardedVideoPresenter(musicControlProvider, rewardedVideoCoolDowns, rewardedVideoAnalytics);
		}
	}
}
