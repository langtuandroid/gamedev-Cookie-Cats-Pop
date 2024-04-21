using System;
using JetBrains.Annotations;


namespace TactileModules.Ads
{
	public class RewardedVideoCommandHandler : BaseCommandHandler
	{
		public static void InjectDependencies(IRewardedVideoPresenter rewardedVideoPresenter)
		{
			RewardedVideoCommandHandler.videoPresenter = rewardedVideoPresenter;
		}

		[UsedImplicitly]

		private static void RequestRewardedVideo()
		{
			RewardedVideoCommandHandler.videoPresenter.RequestVideo();
		}

		[UsedImplicitly]

		private static void ShowRewardedVideo()
		{
			RewardedVideoParameters data = new RewardedVideoParameters("Debug", "commandHandler", 0);
			FiberCtrl.Pool.Run(RewardedVideoCommandHandler.videoPresenter.ShowRewardedVideo(data, delegate(bool x)
			{
			}), false);
		}

		private static IRewardedVideoPresenter videoPresenter;
	}
}
