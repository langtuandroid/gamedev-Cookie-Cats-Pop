using System;
using TactileModules.Ads.Configuration;
using TactileModules.InstallTimeTracking;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
	public class InterstitialSystemBuilder
	{
		public static InterstitialPresenter Build(IAdConfigurationProvider adConfigurationProvider, IPayingUserProvider payingUserProvider, IInstallTime installTime)
		{
			PlayerPrefsSignedString localStorageString = new PlayerPrefsSignedString("Interstitial", "ProviderManagerData");
			LocalStorageJSONObject<InterstitialProviderManagerData> storage = new LocalStorageJSONObject<InterstitialProviderManagerData>(localStorageString);
			TactileDateTime dateTimeGetter = new TactileDateTime();
			InterstitialPresenter interstitialPresenter = new InterstitialPresenter(storage, dateTimeGetter);
			InterstitialTimeRequirement interstitialRequirement = new InterstitialTimeRequirement(adConfigurationProvider, installTime, dateTimeGetter);
			interstitialPresenter.RegisterRequirement(interstitialRequirement);
			InterstitialNotPayingUserRequirement interstitialRequirement2 = new InterstitialNotPayingUserRequirement(payingUserProvider);
			interstitialPresenter.RegisterRequirement(interstitialRequirement2);
			return interstitialPresenter;
		}
	}
}
