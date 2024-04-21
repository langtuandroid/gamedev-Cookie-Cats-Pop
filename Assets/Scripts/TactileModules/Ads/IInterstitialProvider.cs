using System;
using System.Collections;

namespace TactileModules.Ads
{
	public interface IInterstitialProvider
	{
		bool IsInterstitialAvailable { get; }

		bool IsRequestingInterstitial { get; }

		void RequestInterstitial();

		int Priority { get; }

		IEnumerator ShowInterstitial();
	}
}
