using System;
using System.Collections;
using System.Diagnostics;
using Tactile;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.Foundation;
using TactileModules.PuzzleGame.MainLevels;
using TactileModules.PuzzleGames.StarTournament;
using TactileModules.PuzzleGames.StarTournament.Views;
using TactileModules.SagaCore;
using UnityEngine;

public class StarTournamentManagerProvider : IStarTournamentProvider
{
	public StarTournamentManagerProvider()
	{
		ManagerRepository.Get<GameEventManager>().OnGameEvent += this.OnGameEvent;
		ManagerRepository.Get<GameSessionManager>().NewSessionStarted += this.OnNewSessionStarted;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<LevelProxy> OnLevelCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<LevelProxy> OnLevelStarted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnNewSessionStarted;

	private UIViewManager ViewManager
	{
		get
		{
			return ManagerRepository.Get<UIViewManager>();
		}
	}

	private LevelDatabaseCollection LevelDatabaseCollection
	{
		get
		{
			return ManagerRepository.Get<LevelDatabaseCollection>();
		}
	}

	private MainLevelDatabase MainLevelDatabase
	{
		get
		{
			return this.LevelDatabaseCollection.GetLevelDatabase<MainLevelDatabase>("Main");
		}
	}

	private void OnGameEvent(GameEvent gameEvent)
	{
		if (gameEvent.type == 24)
		{
			LevelProxy level = this.MainLevelDatabase.GetLevel((int)gameEvent.context);
			if (this.OnLevelStarted != null)
			{
				this.OnLevelStarted(level);
			}
		}
		else if (gameEvent.type == 25)
		{
			LevelProxy level2 = this.MainLevelDatabase.GetLevel((int)gameEvent.context);
			if (this.OnLevelCompleted != null)
			{
				this.OnLevelCompleted(level2);
			}
		}
	}

	public IEnumerator ShowStarTournamentStartedView(bool isReminder)
	{
		UIViewManager.UIViewState view = this.ViewManager.ShowView<StarTournamentStartView>(new object[]
		{
			isReminder
		});
		yield return view.WaitForClose();
		if (view.ClosingResult is UIViewManager.UIViewState)
		{
			yield return ((UIViewManager.UIViewState)view.ClosingResult).WaitForClose();
		}
		yield break;
	}

	public IEnumerator ShowStarTournamentEndedViewForOld(OldStartTournamentInfo info)
	{
		int coins = info.RewardForOldTournament.Items[0].Amount;
		foreach (ItemAmount itemAmount in info.RewardForOldTournament.Items)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "StarTournamentReward");
		}
		UIViewManager.UIViewStateGeneric<MessageBoxView> view = this.ViewManager.ShowView<MessageBoxView>(new object[]
		{
			L.Get("Congratulations!"),
			string.Format(L.Get("You took a {0} place at previous Star Tournament and won {1} coins"), info.RewardForOldTournament.Rank, coins),
			L.Get("Ok")
		});
		yield return view.WaitForClose();
		yield break;
	}

	public IEnumerator ShowStarTournamentEndedView(StarTournamentConfig.Reward reward, FeatureData featureData)
	{
		UIViewManager.UIViewState view;
		if (reward == null)
		{
			view = this.ViewManager.ShowView<StarTournamentLeaderboardView>(new object[0]);
		}
		else
		{
			view = this.ViewManager.ShowView<StarTournamentRewardView>(new object[]
			{
				reward,
				reward.Rank
			});
		}
		yield return view.WaitForClose();
		yield break;
	}

	public IEnumerator ShowLeaderboard()
	{
		UIViewManager.UIViewStateGeneric<StarTournamentLeaderboardView> vs = this.ViewManager.ShowView<StarTournamentLeaderboardView>(new object[0]);
		yield return vs.WaitForClose();
		yield break;
	}

	public SideButtonsArea GetSideButtonsArea()
	{
		MainMapButtonsView mainMapButtonsView = UIViewManager.Instance.FindView<MainMapButtonsView>();
		return mainMapButtonsView.GetComponentInChildren<SideButtonsArea>();
	}

	public GameObject GetStarPrefab()
	{
		return Resources.Load<GameObject>("StarTournament/Star");
	}

	public int GetStarsForLevel(int levelId)
	{
		return this.MainLevelDatabase.GetLevel(levelId).Stars;
	}

	public StarTournamentConfig Config()
	{
		return ConfigurationManager.Get<StarTournamentConfig>();
	}

	public string GetTextForNotification(TimeSpan timeSpan, ActivatedFeatureInstanceData instanceData)
	{
		return string.Format(L.Get("There are only {0} hours left of the Star Tournament!"), timeSpan.TotalHours);
	}
}
