using System;

namespace TactileModules.Ads
{
	public interface IRewardedVideoAnalytics
	{
		void LogVideoImpression(RewardedVideoParameters data, string adProvider);

		void LogVideoCompleted(RewardedVideoParameters data, string adProvider);
	}
}
