using System;
using System.Collections;
using TactileModules.Ads.Analytics;
using TactileModules.Ads.RewardedVideo;

namespace TactileModules.Ads
{
	public interface IRewardedVideoPresenter
	{
		void RegisterRewardedVideoProvider(IRewardedVideoProvider provider);

		bool IsPlacementEnabled(AdGroupContext adGroupContext);

		bool IsInCooldown(AdGroupContext adGroupContext);

		string GetTimeLeftString(AdGroupContext adGroupContext);

		bool CanShowRewardedVideo(AdGroupContext adGroupContext);

		bool IsRequestingVideo();

		bool IsPlayingVideo();

		void RequestVideo();

		void RegisterRequirement(IRewardedVideoRequirement rewardedVideoRequirement);

		IEnumerator ShowRewardedVideo(RewardedVideoParameters data, Action<bool> videoCompleted);

		IEnumerator FetchAndShowRewardedVideo(RewardedVideoParameters data, Action<bool> videoCompleted, int timeoutSeconds);
	}
}
