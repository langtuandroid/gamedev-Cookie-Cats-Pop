using System;
using System.Collections;

namespace TactileModules.Ads
{
	public interface IInterstitialPresenter
	{
		void Register(IInterstitialProvider provider);

		bool IsInterstitialAvailable { get; }

		void RegisterRequirement(IInterstitialRequirement interstitialRequirement);

		void RequestInterstitial();

		IEnumerator ShowInterstitial();

		bool InterstitialRequirementsAreMet();

		IEnumerator FetchAndShowInterstitial(int timeoutSeconds);
	}
}
