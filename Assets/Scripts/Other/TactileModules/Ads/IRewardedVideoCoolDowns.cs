using System;

namespace TactileModules.Ads
{
	public interface IRewardedVideoCoolDowns
	{
		bool IsPlacementEnabled(string rewardedVideoPlacement);

		void VideoWasCompleted(string rewardedVideoPlacement);

		bool IsInCooldown(string rewardedVideoPlacement);

		string GetTimeLeftString(string rewardedVideoPlacement);
	}
}
