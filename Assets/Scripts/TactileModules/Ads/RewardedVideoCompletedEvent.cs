using System;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads
{
	[TactileAnalytics.EventAttribute("rewardedVideoCompleted", true)]
	public class RewardedVideoCompletedEvent : RewardedVideoBasicEvent
	{
		public RewardedVideoCompletedEvent(RewardedVideoParameters parameters, string adProvider) : base(parameters, adProvider)
		{
		}
	}
}
