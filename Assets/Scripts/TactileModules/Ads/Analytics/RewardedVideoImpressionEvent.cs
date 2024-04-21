using System;

namespace TactileModules.Ads.Analytics
{
	[TactileAnalytics.EventAttribute("rewardedVideoImpression", true)]
	public class RewardedVideoImpressionEvent : RewardedVideoBasicEvent
	{
		public RewardedVideoImpressionEvent(RewardedVideoParameters parameters, string adProvider) : base(parameters, adProvider)
		{
		}
	}
}
