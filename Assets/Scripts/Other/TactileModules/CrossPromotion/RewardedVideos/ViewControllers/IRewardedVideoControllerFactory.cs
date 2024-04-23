using System;

namespace TactileModules.CrossPromotion.RewardedVideos.ViewControllers
{
	public interface IRewardedVideoControllerFactory
	{
		IRewardedVideoViewController CreateViewController();
	}
}
