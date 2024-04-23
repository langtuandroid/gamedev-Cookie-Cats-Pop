using System;
using System.Collections;
using TactileModules.FeatureManager;
using TactileModules.Foundation;
using TactileModules.PuzzleGames.LevelDash;
using TactileModules.PuzzleGames.LevelDash.Providers;
using TactileModules.PuzzleGames.LevelDash.Views;
using UnityEngine;

public class LevelDashViewProvider : ILevelDashViewProvider
{
	private LevelDashManager LevelDashManager
	{
		get
		{
			return FeatureManager.GetFeatureHandler<LevelDashManager>();
		}
	}

	private FacebookClient FacebookClient
	{
		get
		{
			return ManagerRepository.Get<FacebookClient>();
		}
	}

	private CloudClient CloudClient
	{
		get
		{
			return ManagerRepository.Get<CloudClient>();
		}
	}

	public IEnumerator ShowRewardView(LevelDashConfig.Reward reward)
	{
		UIViewManager.UIViewStateGeneric<LevelDashRewardView> vs = UIViewManager.Instance.ShowView<LevelDashRewardView>(new object[]
		{
			reward
		});
		yield return vs.WaitForClose();
		yield break;
	}

	public IEnumerator ShowRewardViewForPreviousLevelDash(int rank, LevelDashConfig.Reward reward)
	{
		object vs = PuzzleGame.DialogViews.ShowMessageBox(L.Get("Congratulation"), string.Format(L.Get("You took a {0} place at previous Level Dash and won a prize"), rank), L.Get("Ok"), null);
		yield return PuzzleGame.DialogViews.WaitForClosingView(vs);
		yield break;
	}

	public void InitializeLeaderboardItem(Entry entry, GameObject go)
	{
		CCPLevelDashLeaderboardItem component = go.GetComponent<CCPLevelDashLeaderboardItem>();
		string deviceId = this.LevelDashManager.GetDeviceId();
		string userId = this.LevelDashManager.GetUserId();
		CloudUser cloudUser = this.LevelDashManager.GetCloudUser(entry.UserId);
		int levelProgression = entry.MaxLevel - this.LevelDashManager.CustomInstanceData.StartLevel;
		int rewardsCount = this.LevelDashManager.GetRewardsCount();
		bool isMe = entry.IsOwnedByDeviceOrUser(deviceId, userId);
		int rank = this.LevelDashManager.GetRank(entry.DeviceId, entry.UserId);
		component.Init(rank, entry, cloudUser, this.FacebookClient, this.CloudClient, isMe, rewardsCount, levelProgression);
	}
}
