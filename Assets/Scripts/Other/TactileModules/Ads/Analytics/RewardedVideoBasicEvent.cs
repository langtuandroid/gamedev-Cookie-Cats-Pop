using System;

namespace TactileModules.Ads.Analytics
{
	public class RewardedVideoBasicEvent : BasicEvent
	{
		public RewardedVideoBasicEvent(RewardedVideoParameters parameters, string adProvider)
		{
			this.Location = parameters.AdGroupContext.ToString();
			this.RewardType = parameters.RewardType;
			this.RewardAmount = parameters.RewardAmount;
			this.AdProvider = adProvider;
		}

		private TactileAnalytics.RequiredParam<string> Location { get; set; }

		private TactileAnalytics.RequiredParam<string> RewardType { get; set; }

		private TactileAnalytics.RequiredParam<int> RewardAmount { get; set; }

		private TactileAnalytics.RequiredParam<string> AdProvider { get; set; }
	}
}
