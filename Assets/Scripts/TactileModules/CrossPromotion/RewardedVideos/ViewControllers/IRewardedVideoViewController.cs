using System;
using System.Collections;
using TactileModules.Ads.Analytics;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewControllers
{
	public interface IRewardedVideoViewController
	{
		bool ShowViewIfPossible(AdGroupContext adGroupContext);

		IEnumerator WaitForClose();

		bool WasVideoAborted();

		bool ShouldRewatchVideo();

		bool IsShowing();
	}
}
