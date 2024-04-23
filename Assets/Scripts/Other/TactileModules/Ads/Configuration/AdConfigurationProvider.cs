using System;
using TactileModules.Ads.Analytics;
using TactileModules.PuzzleGames.Configuration;
using TactileModules.TactileCloud;

namespace TactileModules.Ads.Configuration
{
	public class AdConfigurationProvider : IAdConfigurationProvider
	{
		public AdConfigurationProvider(ICloudClientState cloudClient, IConfigurationManager configManager, IAnalyticsAdjustAttribution analytics)
		{
			this.cloudClient = cloudClient;
			this.configManager = configManager;
			this.analytics = analytics;
			this.SelectConfiguration();
			analytics.AdjustAttributionChanged += this.SelectConfiguration;
			configManager.ConfigurationUpdated += this.SelectConfiguration;
		}

		public AdConfiguration ActiveConfiguration
		{
			get
			{
				if (this.activeConfiguration == null)
				{
					this.activeConfiguration = this.GetAdConfigContainer().Default;
				}
				return this.activeConfiguration;
			}
		}

		private AdConfigContainer GetAdConfigContainer()
		{
			return this.configManager.GetConfig<AdConfigContainer>();
		}

		public void SelectConfiguration()
		{
			
		}

		public RewardedVideoContextConfiguration GetRewardedVideoPlacementConfig(AdGroupContext adGroupContext)
		{
			foreach (RewardedVideoContextConfiguration rewardedVideoContextConfiguration in this.ActiveConfiguration.RewardedVideoConfiguration.RewardedVideoContextConfiguration)
			{
				if (rewardedVideoContextConfiguration.RewardedVideoContext == adGroupContext)
				{
					return rewardedVideoContextConfiguration;
				}
			}
			return null;
		}

		private readonly ICloudClientState cloudClient;

		private readonly IConfigurationManager configManager;

		private readonly IAnalyticsAdjustAttribution analytics;

		private AdConfiguration activeConfiguration;
	}
}
