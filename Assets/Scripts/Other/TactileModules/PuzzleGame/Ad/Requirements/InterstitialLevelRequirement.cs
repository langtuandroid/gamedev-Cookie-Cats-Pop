using System;
using TactileModules.Ads;
using TactileModules.Ads.Configuration;
using TactileModules.PuzzleGame.MainLevels;

namespace TactileModules.PuzzleGame.Ad.Requirements
{
	public class InterstitialLevelRequirement : IInterstitialRequirement
	{
		public InterstitialLevelRequirement(IAdConfigurationProvider adConfigurationProvider, IMainProgression mainProgression)
		{
			this.adConfigurationProvider = adConfigurationProvider;
			this.mainProgression = mainProgression;
		}

		public bool RequirementIsMet(InterstitialProviderManagerData data)
		{
			int levelRequiredForInterstitials = this.adConfigurationProvider.ActiveConfiguration.InterstitialConfiguration.LevelRequiredForInterstitials;
			int farthestUnlockedLevelIndex = this.mainProgression.GetFarthestUnlockedLevelIndex();
			return farthestUnlockedLevelIndex >= levelRequiredForInterstitials;
		}

		private readonly IAdConfigurationProvider adConfigurationProvider;

		private readonly IMainProgression mainProgression;
	}
}
