using System;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads
{
	public class RewardedVideoParameters
	{
		public RewardedVideoParameters(AdGroupContext adGroupContext, string rewardType, int rewardAmount)
		{
			this.AdGroupContext = adGroupContext;
			this.RewardType = rewardType;
			this.RewardAmount = rewardAmount;
		}

		public AdGroupContext AdGroupContext { get; private set; }

		public string RewardType { get; private set; }

		public int RewardAmount { get; private set; }
	}
}
