using System;
using System.Collections.Generic;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.LevelDash;

public class MainMapLevelDashAvatarsInfoProvider : MapAvatarController.IAvatarsInfoProvider
{
	public MainMapLevelDashAvatarsInfoProvider(LevelDashManager manager)
	{
		this.manager = manager;
		this.entriesWithoutFriends = this.GetAllOtherEntriesWithoutFriends();
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	private List<Entry> GetAllOtherEntriesWithoutFriends()
	{
		List<Entry> allOtherEntries = this.manager.GetAllOtherEntries();
		List<CloudUser> cachedFriends = this.CloudClient.CachedFriends;
		List<Entry> list = new List<Entry>();
		foreach (Entry entry in allOtherEntries)
		{
			bool flag = false;
			foreach (CloudUser cloudUser in cachedFriends)
			{
				if (entry.UserId == cloudUser.CloudId)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(entry);
			}
		}
		return list;
	}

	public int GetAvatarsCount()
	{
		return this.entriesWithoutFriends.Count;
	}

	public int GetDotIndexForAvatar(int index)
	{
		List<Entry> list = this.entriesWithoutFriends;
		if (index < list.Count)
		{
			MainLevelDatabase levelDatabase = ManagerRepository.Get<LevelDatabaseCollection>().GetLevelDatabase<MainLevelDatabase>("Main");
			return levelDatabase.NonGateIndexToDotIndex(levelDatabase.GetLevel(list[index].MaxLevel - 1), typeof(GateMetaData));
		}
		return -1;
	}

	public int GetAvatarFramePrefabIndex()
	{
		return 1;
	}

	public int GetExtraAvatarFramePrefabIndex()
	{
		return 1;
	}

	public CloudUser GetCloudUser(int index)
	{
		List<Entry> list = this.entriesWithoutFriends;
		if (index < list.Count)
		{
			Entry entry = list[index];
			return this.manager.GetCloudUser(entry.UserId);
		}
		return null;
	}

	public string GetDeviceId(int index)
	{
		List<Entry> list = this.entriesWithoutFriends;
		if (index < list.Count)
		{
			Entry entry = list[index];
			return entry.DeviceId;
		}
		return null;
	}

	private readonly LevelDashManager manager;

	private readonly List<Entry> entriesWithoutFriends;
}
