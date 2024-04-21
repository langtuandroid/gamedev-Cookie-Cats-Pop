using System;
using TactileModules.Ads.Analytics;
using TactileModules.Ads.Configuration;
using TactileModules.Ads.RewardedVideo;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.PuzzleGame.Ad.Requirements
{
	public class RewardedVideoLevelRequirement : IRewardedVideoRequirement
	{
		public RewardedVideoLevelRequirement(IAdConfigurationProvider adConfigurationProvider, IMainProgression mainProgression)
		{
			this.adConfigurationProvider = adConfigurationProvider;
			this.mainProgression = mainProgression;
		}

		public bool MeetsRequirement(AdGroupContext adGroupContext)
		{
			int farthestUnlockedLevelHumanNumber = this.mainProgression.GetFarthestUnlockedLevelHumanNumber();
			RewardedVideoContextConfiguration rewardedVideoPlacementConfig = this.adConfigurationProvider.GetRewardedVideoPlacementConfig(adGroupContext);
			if (rewardedVideoPlacementConfig != null)
			{
				int levelRequired = rewardedVideoPlacementConfig.LevelRequired;
				return farthestUnlockedLevelHumanNumber >= levelRequired;
			}
			return false;
		}

		private readonly IAdConfigurationProvider adConfigurationProvider;

		private readonly IMainProgression mainProgression;
	}
}
