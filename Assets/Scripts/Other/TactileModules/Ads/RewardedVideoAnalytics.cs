using System;
using TactileModules.Ads.Analytics;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Ads
{
	public class RewardedVideoAnalytics : IRewardedVideoAnalytics
	{
		public RewardedVideoAnalytics(IAnalytics tactileAnalytics)
		{
			this.tactileAnalytics = tactileAnalytics;
		}

		public void LogVideoImpression(RewardedVideoParameters data, string adProvider)
		{
			this.tactileAnalytics.LogEvent(new RewardedVideoImpressionEvent(data, adProvider), -1.0, null);
		}

		public void LogVideoCompleted(RewardedVideoParameters data, string adProvider)
		{
			this.tactileAnalytics.LogEvent(new RewardedVideoCompletedEvent(data, adProvider), -1.0, null);
		}

		private readonly IAnalytics tactileAnalytics;
	}
}
