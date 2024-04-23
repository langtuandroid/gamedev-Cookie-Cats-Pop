using System;
using TactileModules.Ads;
using TactileModules.Ads.Configuration;
using TactileModules.PuzzleGame.Ad.Requirements;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.PuzzleGame.Ads
{
	public class AdManagerSystemBuilder
	{
		public static void Build(AdConfigurationProvider adConfigurationProvider, MainProgressionManager mainProgressionManager, IRewardedVideoPresenter rewardedVideoPresenter, IInterstitialPresenter interstitialPresenter)
		{
			InterstitialLevelRequirement interstitialRequirement = new InterstitialLevelRequirement(adConfigurationProvider, mainProgressionManager);
			interstitialPresenter.RegisterRequirement(interstitialRequirement);
			RewardedVideoLevelRequirement rewardedVideoRequirement = new RewardedVideoLevelRequirement(adConfigurationProvider, mainProgressionManager);
			rewardedVideoPresenter.RegisterRequirement(rewardedVideoRequirement);
		}
	}
}
