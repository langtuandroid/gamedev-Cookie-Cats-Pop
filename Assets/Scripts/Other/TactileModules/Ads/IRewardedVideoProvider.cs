using System;
using System.Collections;
using TactileModules.Ads.Analytics;

namespace TactileModules.Ads
{
	public interface IRewardedVideoProvider
	{
		bool IsVideoAvailable { get; }

		bool IsPreparingVideo { get; }

		void RequestVideo();

		int Priority { get; }

		string AdProviderDisplayName { get; }

		IEnumerator ShowVideo(AdGroupContext adGroupContext, Action videoStarted, Action<bool> videoCompleted);
	}
}
