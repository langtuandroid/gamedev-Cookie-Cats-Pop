using System;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;

public class SlidesAndLaddersMapViewProvider : ISlidesAndLaddersMapViewProvider
{
	public LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	public MapStreamerCollection MapStreamerCollection
	{
		get
		{
			return ManagerRepository.Get<MapStreamerCollection>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public void SubscribeToFriendsAndSettingsSynced(Action callback)
	{
		this.CloudClient.FriendsAndSettingsSynced += callback;
	}

	public void UnsubscribeToFriendsAndSettingsSynced(Action callback)
	{
		this.CloudClient.FriendsAndSettingsSynced -= callback;
	}
	
}
