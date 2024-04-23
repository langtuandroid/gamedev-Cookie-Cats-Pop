using System;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
	public class RewardedVideoStoreFactory : IRewardedVideoStoreFactory
	{
		public ILocalStorageString CreatePlayerPrefsSecuredString(string rewardedVideoPlacementIdentifier)
		{
			return new PlayerPrefsSignedString("RewardedVideoStore", "WATCH_VIDEO_COOLDOWN" + rewardedVideoPlacementIdentifier);
		}

		private const string PERSISTED_VIDEO_SHOWN_ID = "WATCH_VIDEO_COOLDOWN";

		private const string REWARDED_VIDEO_STORE = "RewardedVideoStore";
	}
}
