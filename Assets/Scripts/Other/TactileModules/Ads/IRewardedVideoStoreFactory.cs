using System;
using TactileModules.TactilePrefs;

namespace TactileModules.Ads
{
	public interface IRewardedVideoStoreFactory
	{
		ILocalStorageString CreatePlayerPrefsSecuredString(string rewardedVideoPlacementIdentifier);
	}
}
