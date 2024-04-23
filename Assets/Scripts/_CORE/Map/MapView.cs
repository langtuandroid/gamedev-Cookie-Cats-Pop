using System;
using TactileModules.Foundation;

public abstract class MapView : MapViewBase
{
	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	protected override void SubscribeToFriendsAndSettingsSynced(Action callback)
	{
		this.CloudClient.FriendsAndSettingsSynced += callback;
	}

	protected override void UnsubscribeToFriendsAndSettingsSynced(Action callback)
	{
		this.CloudClient.FriendsAndSettingsSynced -= callback;
	}

	protected override void SubscribeToVIPStateChange(Action<bool> callback)
	{
		VipManager instance = VipManager.Instance;
		instance.VipStateChanged = (Action<bool>)Delegate.Combine(instance.VipStateChanged, callback);
	}

	protected override void UnsubscribeToVIPStateChange(Action<bool> callback)
	{
		VipManager instance = VipManager.Instance;
		instance.VipStateChanged = (Action<bool>)Delegate.Remove(instance.VipStateChanged, callback);
	}

	protected abstract string LevelType { get; }

	protected override bool LastPlayedLevelBelongsToCurrentMap()
	{
		return base.lastLevelPlayedType == this.LevelType;
	}

	protected override LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	protected override MapStreamerCollection MapStreamerCollection
	{
		get
		{
			return ManagerRepository.Get<MapStreamerCollection>();
		}
	}
}
