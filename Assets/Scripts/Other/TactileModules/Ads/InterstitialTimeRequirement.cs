using System;
using TactileModules.Ads.Configuration;
using TactileModules.InstallTimeTracking;
using TactileModules.RuntimeTools;

namespace TactileModules.Ads
{
	public class InterstitialTimeRequirement : IInterstitialRequirement
	{
		public InterstitialTimeRequirement(IAdConfigurationProvider adConfigurationProvider, IInstallTime installTime, ITactileDateTime dateTimeGetter)
		{
			this.adConfigurationProvider = adConfigurationProvider;
			this.installTime = installTime;
			this.dateTimeGetter = dateTimeGetter;
		}

		public bool RequirementIsMet(InterstitialProviderManagerData data)
		{
			return this.IsAfterNoAdsStartPeriod() && this.IsCooldownBetweenAdsOver(data);
		}

		private bool IsAfterNoAdsStartPeriod()
		{
			return this.installTime.GetSecondsSinceFirstInstall() >= this.adConfigurationProvider.ActiveConfiguration.InterstitialConfiguration.MinimumSecondsBeforeFirstAd;
		}

		private bool IsCooldownBetweenAdsOver(InterstitialProviderManagerData data)
		{
			DateTime utcNow = this.dateTimeGetter.UtcNow;
			return (utcNow - data.LastInterstitialShown).TotalSeconds >= (double)this.adConfigurationProvider.ActiveConfiguration.InterstitialConfiguration.MinimumSecondsBetweenInterstitials;
		}

		private readonly IAdConfigurationProvider adConfigurationProvider;

		private readonly IInstallTime installTime;

		private readonly ITactileDateTime dateTimeGetter;
	}
}
