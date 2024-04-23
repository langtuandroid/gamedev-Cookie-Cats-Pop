using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;

public class MainMapFBFriendsAvatarsInfoProvider : MapAvatarController.IAvatarsInfoProvider
{
	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public int GetAvatarsCount()
	{
		return this.CloudClient.CachedFriends.Count;
	}

	public string GetDeviceId(int index)
	{
		return null;
	}

	public int GetDotIndexForAvatar(int index)
	{
		CloudUser cloudUser = this.GetCloudUser(index);
		if (cloudUser != null)
		{
			MainProgressionManager.PublicState friend = UserSettingsManager.GetFriend<MainProgressionManager.PublicState>(cloudUser);
			if (friend != null)
			{
				return friend.LatestUnlockedIndex;
			}
		}
		return -1;
	}

	public int GetAvatarFramePrefabIndex()
	{
		return 0;
	}

	public int GetExtraAvatarFramePrefabIndex()
	{
		return 0;
	}

	public CloudUser GetCloudUser(int index)
	{
		List<CloudUser> cachedFriends = this.CloudClient.CachedFriends;
		if (index < cachedFriends.Count)
		{
			return cachedFriends[index];
		}
		return null;
	}
}
