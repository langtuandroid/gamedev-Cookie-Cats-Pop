using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.LevelDash;
using UnityEngine;

public class MapAvatarControllerDataRetriever : MapAvatarController.IDataRetriever
{
	public MapAvatarControllerDataRetriever(MapStreamer mapStreamer, Func<int> getFarthestUnlockedLevelFunction)
	{
		this.mapStreamer = mapStreamer;
		this.getFarthestUnlockedLevelFunction = getFarthestUnlockedLevelFunction;
		this.avatarLists = new List<MapAvatarController.IAvatarsInfoProvider>
		{
			new MainMapFBFriendsAvatarsInfoProvider(),
			new MainMapLevelDashAvatarsInfoProvider(FeatureManager.GetFeatureHandler<LevelDashManager>())
		};
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public List<MapAvatarController.IAvatarsInfoProvider> GetAvatarsList
	{
		get
		{
			return this.avatarLists;
		}
	}

	public Vector3 GetDotPosition(int dotIndex)
	{
		Vector3 result;
		this.mapStreamer.TryGetDotPosition(dotIndex, out result);
		return result;
	}

	public bool GetVIPStatusForCloudUser(CloudUser cloudUser)
	{
		if (cloudUser == null)
		{
			return false;
		}
		PublicUserSettings friend = UserSettingsManager.GetFriend<PublicUserSettings>(cloudUser);
		return friend != null && friend.UserIsVip;
	}

	public CloudUser GetCachedMe
	{
		get
		{
			return this.CloudClient.CachedMe;
		}
	}

	public string GetCloudDeviceId
	{
		get
		{
			return (!this.CloudClient.HasValidDevice) ? string.Empty : this.CloudClient.CachedDevice.CloudId;
		}
	}

	public bool IsPlayerVIP
	{
		get
		{
			return VipManager.Instance.UserIsVip();
		}
	}

	public AnimationCurve GetMovementCurve
	{
		get
		{
			return SingletonAsset<MapVisualSettings>.Instance.avatarMoveCurve;
		}
	}

	public int GetFarthestUnlockedLevel
	{
		get
		{
			return this.getFarthestUnlockedLevelFunction();
		}
	}

	public bool CanShowOtherAvatars
	{
		get
		{
			return false;
		}
	}

	public Transform GetAvatarRoot
	{
		get
		{
			return this.mapStreamer.GetComponent<UIScrollablePanel>().ScrollRoot;
		}
	}

	private MapStreamer mapStreamer;

	private Func<int> getFarthestUnlockedLevelFunction;

	private readonly List<MapAvatarController.IAvatarsInfoProvider> avatarLists;
}
