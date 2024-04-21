using System;
using System.Collections;
using System.Collections.Generic;
using Tactile;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.GameCore;

public class DailyQuestManagerProvider : PuzzleGameImplementation.PlayerStateProvider, DailyQuestManager.IDailyQuestManagerInterface, IPlayerState
{
	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public void RegisterUnlockPopup(DailyQuestManager manager)
	{
		MapPopupManager.Instance.RegisterPopupObject(manager);
	}

	public IEnumerator ShowUnlockPopup()
	{
		yield return UIViewManager.Instance.ShowView<DailyQuestUnlockedView>(new object[0]).WaitForClose();
		IDailyQuestSystem dailyQuestSystem = ManagerRepository.Get<IDailyQuestSystem>();
		if (dailyQuestSystem.Manager.IsChallengeAvailable)
		{
			FlowStack flowStack = ManagerRepository.Get<FlowStack>();
			DailyQuestMapFlow c = dailyQuestSystem.Factory.CreateMapFlow();
			flowStack.Push(c);
		}
		yield break;
	}

	public DailyQuestManager.PersistableState PersistableState
	{
		get
		{
			return UserSettingsManager.Instance.GetSettings<DailyQuestManager.PersistableState>();
		}
	}

	public List<CloudUser> CachedFriends
	{
		get
		{
			return this.CloudClient.CachedFriends;
		}
	}

	public DailyQuestConfig Config
	{
		get
		{
			return ConfigurationManager.Get<DailyQuestConfig>();
		}
	}

	public int CompletedQuestIndexForFriend(CloudUser friend)
	{
		DailyQuestManager.PublicState friendSettings = UserSettingsManager.Instance.GetFriendSettings<DailyQuestManager.PublicState>(friend);
		if (friendSettings != null)
		{
			return friendSettings.CompletedQuestNumber;
		}
		return 0;
	}

	public CloudUser GetCloudUserForFacebookId(string facebookId)
	{
		return this.CloudClient.GetUserForFacebookId(facebookId);
	}

	public List<DailyQuestInfo> DayItems
	{
		get
		{
			return ConfigurationManager.Get<DailyQuestConfig>().Quests;
		}
	}

	public void SaveLocalSettings()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public void SyncUserSettings()
	{
		ManagerRepository.Get<CloudSynchronizer>().SyncCloud();
	}

	public void SavePublicCompletedQuestIndex(int index)
	{
		UserSettingsManager.Instance.GetSettings<DailyQuestManager.PublicState>().CompletedQuestNumber = index;
	}
}
