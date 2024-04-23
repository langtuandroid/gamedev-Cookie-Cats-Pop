using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile;
using TactileModules.PuzzleGames.Lives;
using UnityEngine;

public class TournamentManager
{
	private TournamentManager(TournamentManager.ITournamentUIProvider uiProvider, TournamentCloudManager tournamentCloudManager, LevelDatabaseCollection levelDatabaseCollection, MapStreamerCollection mapStreamerCollection, TimeStampManager timeStampManager, ILivesManager livesManager)
	{
		this.tournamentCloudManager = tournamentCloudManager;
		this.uiProvider = uiProvider;
		this.regeneratingTournamentLives = new RegeneratingItemService(timeStampManager, "TournamentLife", this.Config.LifeRegenerationMaxCount, this.Config.LifeRegenerationTime);
		this.levelDatabaseCollection = levelDatabaseCollection;
		this.livesManager = livesManager;
		levelDatabaseCollection.RegisterLevelDatabaseListener("Tournament");
		mapStreamerCollection.RegisterLevelDatabaseListener("Tournament");
	}

	public int BronzeTickets
	{
		get
		{
			return InventoryManager.Instance.GetAmount("TicketBronze");
		}
	}

	public int SilverTickets
	{
		get
		{
			return InventoryManager.Instance.GetAmount("TicketSilver");
		}
	}

	public int GoldTickets
	{
		get
		{
			return InventoryManager.Instance.GetAmount("TicketGold");
		}
	}

	public int Lives
	{
		get
		{
			return InventoryManager.Instance.GetAmount("TournamentLife");
		}
	}

	public int PlayingDotId { get; set; }

	public bool UserWillBeNotifiedWhenTournamentIsJoinable { get; private set; }

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action WasReset;



	public static TournamentManager Instance { get; private set; }

	public TournamentCloudManager Cloud
	{
		get
		{
			return this.tournamentCloudManager;
		}
	}

	public TournamentLevelDatabase GetLevelDatabase()
	{
		return this.levelDatabaseCollection.GetLevelDatabase<TournamentLevelDatabase>("Tournament");
	}

	public static TournamentManager CreateInstance(TournamentManager.ITournamentUIProvider uiProvider, TournamentCloudManager tournamentCloudManager, LevelDatabaseCollection levelDatabaseCollection, MapStreamerCollection mapStreamerCollection, TimeStampManager timeStampManager, ILivesManager livesManager)
	{
		TournamentManager.Instance = new TournamentManager(uiProvider, tournamentCloudManager, levelDatabaseCollection, mapStreamerCollection, timeStampManager, livesManager);
		return TournamentManager.Instance;
	}

	private IPlayerState playerProgression
	{
		get
		{
			return PuzzleGame.PlayerState;
		}
	}

	private IDialogViewProvider GameDialogs
	{
		get
		{
			return PuzzleGame.DialogViews;
		}
	}

	public bool IsTournamentRankUnlocked(TournamentRank rank)
	{
		return this.playerProgression.FarthestUnlockedLevelHumanNumber >= this.LevelNrRequiredForTournament(rank);
	}

	public int LevelNrRequiredForTournament(TournamentRank rank)
	{
		if (rank == TournamentRank.Bronze)
		{
			return this.Config.LevelNrRequiredForTournament;
		}
		if (rank == TournamentRank.Silver)
		{
			return this.Config.LevelNrRequiredForSilverTournament;
		}
		if (rank != TournamentRank.Gold)
		{
			return int.MaxValue;
		}
		return this.Config.LevelNrRequiredForGoldTournament;
	}

	public bool UserCompletedTournament
	{
		get
		{
			return this.IsDotCompleted(5);
		}
	}

	public TournamentRankConfig GetRewardsForTournamentRank(TournamentRank rank)
	{
		return this.Config.GetRankConfig(rank);
	}

	public TournamentPrizeConfig GetPrizeForRankAndPosition(TournamentRank rank, int position)
	{
		TournamentRankConfig rankConfig = this.Config.GetRankConfig(rank);
		return rankConfig.Prizes.FindLast((TournamentPrizeConfig p) => p.RankFrom <= position);
	}

	public bool HasTicketsForTournament(int neededAmount, TournamentRank rank)
	{
		return this.GetTicketsForRank(rank) >= neededAmount;
	}

	public void SetPersistedTournamentUnlocked()
	{
		this.GetState().TournamentUnlocked = 1;
		this.Save();
	}

	public bool GetPersistedTournamentUnlocked()
	{
		return this.GetState().TournamentUnlocked > 0;
	}

	public void AddTicketsToUser(TournamentRank rank, int amount, string contextTag)
	{
		InventoryItem item;
		if (this.TryGetInventoryItemForRank(rank, out item))
		{
			InventoryManager.Instance.Add(item, amount, contextTag);
		}
	}

	private void RemoveTicketFromUser(int amount, TournamentRank rank, string contextTag)
	{
		InventoryItem item;
		if (this.TryGetInventoryItemForRank(rank, out item))
		{
			InventoryManager.Instance.Consume(item, amount, contextTag);
		}
	}

	private bool TryGetInventoryItemForRank(TournamentRank rank, out InventoryItem item)
	{
		switch (rank)
		{
		case TournamentRank.Bronze:
			item = "TicketBronze";
			return true;
		case TournamentRank.Silver:
			item = "TicketSilver";
			return true;
		case TournamentRank.Gold:
			item = "TicketGold";
			return true;
		default:
			item = null;
			return false;
		}
	}

	public int GetTicketsForRank(TournamentRank rank)
	{
		InventoryItem item;
		if (this.TryGetInventoryItemForRank(rank, out item))
		{
			return InventoryManager.Instance.GetAmount(item);
		}
		return 0;
	}

	public TournamentRank GetCurrentRank()
	{
		if (this.tournamentCloudManager.TournamentJoined)
		{
			return this.tournamentCloudManager.GetJoinedRank;
		}
		return TournamentRank.None;
	}

	public bool HasUnlimitedLives()
	{
		return this.livesManager.HasUnlimitedLives();
	}

	public void UseLife()
	{
		if (this.HasUnlimitedLives())
		{
			return;
		}
		InventoryManager.Instance.Consume("TournamentLife", 1, null);
	}

	public int GetSecondsUntilLifeHasRegenerated()
	{
		return this.regeneratingTournamentLives.GetSecondsLeftForRegeneration();
	}

	public void StartNewTournament(TournamentRank rank)
	{
		this.Save();
	}

	public bool TryClaimRewardAndAddToInventory(out TournamentPrizeConfig prize)
	{
		prize = null;
		if (this.Cloud.TournamentResultPreviouslyPresented)
		{
			return false;
		}
		TournamentRank currentRank = this.GetCurrentRank();
		TournamentCloudManager.Score myOverallScore = this.tournamentCloudManager.GetMyOverallScore();
		GameEventManager.Instance.Emit(1000, this, (int)currentRank);
		prize = this.GetPrizeForRankAndPosition(currentRank, myOverallScore.position);
		foreach (ItemAmount itemAmount in prize.Rewards)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "tournamentReward");
		}
		this.PersistedUnsubmittedScore = null;
		this.Save();
		this.tournamentCloudManager.Reset();
		this.WasReset();
		return true;
	}

	public bool IsDotCompleted(int dotId)
	{
		return this.tournamentCloudManager.GetMyLeaderboardScore(dotId).score > 0;
	}

	public int GetHighestUnlockedLevel()
	{
		for (int i = 0; i < 6; i++)
		{
			if (this.tournamentCloudManager.GetMyLeaderboardScore(i).score == 0)
			{
				return i;
			}
		}
		return 5;
	}

	public int GetScoreForDot(int dotId)
	{
		return this.tournamentCloudManager.GetMyLeaderboardScore(dotId).score;
	}

	public LevelProxy GetLevelFromDotId(int dotId)
	{
		dotId = Mathf.Clamp(dotId, 0, 5);
		return this.GetTournamentLevel(dotId, this.tournamentCloudManager.PeriodId);
	}

	private LevelProxy GetTournamentLevel(int index, int period)
	{
		TournamentLevelDatabase levelDatabase = this.GetLevelDatabase();
		return levelDatabase.GetTournamentLevel(index, period);
	}

	public int GetStarsForDot(int dotId)
	{
		int scoreForDot = this.GetScoreForDot(dotId);
		LevelProxy levelFromDotId = this.GetLevelFromDotId(dotId);
		int num = 0;
		foreach (int num2 in levelFromDotId.StarThresholds)
		{
			if (scoreForDot >= num2)
			{
				num++;
			}
		}
		return num;
	}

	public int GetNextLevel(int dotId)
	{
		int result = -1;
		if (dotId < 5)
		{
			result = dotId + 1;
		}
		return result;
	}

	public string GetTimeRemainingForNextTournamentAsString()
	{
		TimeSpan timeSpan;
		if (!this.Cloud.TournamentOpen)
		{
			DateTime tournamentUtcEndTime = this.tournamentCloudManager.TournamentUtcEndTime;
			timeSpan = tournamentUtcEndTime - DateTime.UtcNow;
			if (timeSpan.TotalMilliseconds < 0.0)
			{
				timeSpan = new TimeSpan(0L);
			}
		}
		else
		{
			timeSpan = new TimeSpan(0L);
		}
		return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public string GetTimeRemainingForTournamentAsString()
	{
		if (!this.tournamentCloudManager.TournamentActive)
		{
			TimeSpan timeSpan = new TimeSpan(0L);
			return timeSpan.ToString();
		}
		TimeSpan timeSpan2 = this.tournamentCloudManager.TournamentUtcEndTime - DateTime.UtcNow;
		if (timeSpan2.TotalMilliseconds <= 0.0)
		{
			timeSpan2 = new TimeSpan(0L);
		}
		if (timeSpan2.Days <= 0)
		{
			return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan2.Hours, timeSpan2.Minutes, timeSpan2.Seconds);
		}
		if (timeSpan2.Days == 1)
		{
			return L._("1 day");
		}
		return string.Format(L._("{0} days"), timeSpan2.Days);
	}

	public string GetTotalScoreForTournament()
	{
		return L.FormatNumber(this.tournamentCloudManager.GetMyOverallScore().score);
	}

	public void NotifyUserWhenTournamentIsJoinable()
	{
		this.uiProvider.ScheduleNotificationWhenTournamentIsJoinable(this.tournamentCloudManager.TournamentUtcEndTime.ToLocalTime());
		this.UserWillBeNotifiedWhenTournamentIsJoinable = true;
	}

	private TournamentManager.PersistableState GetState()
	{
		return UserSettingsManager.Instance.GetSettings<TournamentManager.PersistableState>();
	}

	public void Save()
	{
		UserSettingsManager.Instance.SaveLocalSettings();
	}

	public void DeveloperReset()
	{
		UserSettingsManager.Instance.ResetSettings<TournamentManager.PersistableState>();
	}

	public IEnumerator UpdateStatusOrShowDialog(Action<bool> didSucceed)
	{
		object i;
		do
		{
			LocalScore unsubmittedScore = this.PersistedUnsubmittedScore;
			object error = null;
			if (unsubmittedScore != null)
			{
				object vs = this.GameDialogs.ShowProgressView(L.Get("Submitting Score.."));
				yield return this.tournamentCloudManager.SubmitScore(unsubmittedScore.Leaderboard, unsubmittedScore.Score, delegate(object err)
				{
					error = err;
				});
				this.GameDialogs.CloseView(vs);
			}
			if (!this.Cloud.TournamentJoined && error == null)
			{
				object vs2 = this.GameDialogs.ShowProgressView(L.Get("Loading Tournament.."));
				yield return this.Cloud.UpdateStatus(delegate(object err)
				{
					error = err;
				});
				this.GameDialogs.CloseView(vs2);
			}
			if (error == null && this.Cloud.TournamentValid)
			{
				goto IL_23A;
			}
			i = this.GameDialogs.ShowMessageBox("Tournament", "Could not get tournament data. Try again or come back later", "Try again", "Quit");
			yield return this.GameDialogs.WaitForClosingView(i);
		}
		while ((int)this.GameDialogs.GetViewResult(i) == 0);
		didSucceed(false);
		yield break;
		IL_23A:
		this.PersistedUnsubmittedScore = null;
		didSucceed(true);
		yield break;
		yield break;
	}

	public TournamentCloudManager.Score FindNextPlayerToBeatInfo()
	{
		TournamentCloudManager.Score myOverallScore = this.Cloud.GetMyOverallScore();
		TournamentCloudManager.Score result = null;
		List<TournamentCloudManager.Score> sortedOverallScores = this.Cloud.SortedOverallScores;
		if (sortedOverallScores.Count > 0)
		{
			for (int i = sortedOverallScores.Count - 1; i >= 0; i--)
			{
				if (sortedOverallScores[i].score > myOverallScore.score)
				{
					result = sortedOverallScores[i];
					break;
				}
			}
		}
		return result;
	}

	public TournamentRank GetHighestAvailableTournament()
	{
		TournamentRank tournamentRank;
		if (this.GoldTickets > 0 && this.IsTournamentRankUnlocked(TournamentRank.Gold))
		{
			tournamentRank = TournamentRank.Gold;
		}
		else if (this.SilverTickets > 0 && this.IsTournamentRankUnlocked(TournamentRank.Silver))
		{
			tournamentRank = TournamentRank.Silver;
		}
		else if (this.BronzeTickets > 0 && this.IsTournamentRankUnlocked(TournamentRank.Bronze))
		{
			tournamentRank = TournamentRank.Bronze;
		}
		else
		{
			tournamentRank = TournamentRank.Bronze;
			tournamentRank = ((!this.IsTournamentRankUnlocked(TournamentRank.Silver)) ? tournamentRank : TournamentRank.Silver);
			tournamentRank = ((!this.IsTournamentRankUnlocked(TournamentRank.Gold)) ? tournamentRank : TournamentRank.Gold);
		}
		return tournamentRank;
	}

	public IEnumerator TryJoin(TournamentRank rank, Func<TournamentRank, IEnumerator> showTicketShopIfNoTicket, Action<bool> whenDone)
	{
		if (!this.HasTicketsForTournament(1, rank))
		{
			yield return showTicketShopIfNoTicket(rank);
		}
		if (this.HasTicketsForTournament(1, rank))
		{
			object vs = this.GameDialogs.ShowProgressView(L.Get("Joining Tournament"));
			object error = null;
			yield return this.Cloud.Join((TournamentCloudManager.Type)rank, delegate(object err)
			{
				error = err;
			});
			this.GameDialogs.CloseView(vs);
			yield return this.GameDialogs.WaitForClosingView(vs);
			if (error == null)
			{
				this.StartNewTournament(rank);
				this.RemoveTicketFromUser(1, rank, "JoinTournament" + rank.ToString());
				whenDone(true);
			}
			else
			{
				vs = this.GameDialogs.ShowMessageBox(L.Get("Unable to join"), L.Get("It was not possible to join the tournament right now. Please try again later."), L.Get("Ok"), null);
				yield return this.GameDialogs.WaitForClosingView(vs);
				whenDone(false);
			}
		}
		yield break;
	}

	public IEnumerator StartFlow(TournamentManager.IFlowProvider flowProvider, Action<bool> didSucceed)
	{
		bool didRefresh = false;
		yield return this.UpdateStatusOrShowDialog(delegate(bool success)
		{
			didRefresh = success;
		});
		if (!didRefresh)
		{
			didSucceed(false);
			yield break;
		}
		if (this.Cloud.TournamentJoined && this.Cloud.TournamentEnded)
		{
			yield return this.EndFlow(flowProvider);
			yield return this.UpdateStatusOrShowDialog(delegate(bool success)
			{
				didRefresh = success;
			});
			if (!didRefresh)
			{
				didSucceed(false);
				yield break;
			}
		}
		if (!this.Cloud.TournamentJoined)
		{
			yield return flowProvider.JoinNewTournament();
			if (!this.Cloud.TournamentJoined)
			{
				didSucceed(false);
				yield break;
			}
		}
		didSucceed(true);
		yield break;
	}

	public IEnumerator EndFlow(TournamentManager.IFlowProvider flowProvider)
	{
		if (!this.tournamentCloudManager.IsTournamentResultReady)
		{
			if (this.tournamentCloudManager.TournamentEnded)
			{
				object vs = this.GameDialogs.ShowProgressView(L.Get("Ending Tournament"));
				yield return this.tournamentCloudManager.Update();
				this.GameDialogs.CloseView(vs);
			}
			if (!this.tournamentCloudManager.TournamentValid)
			{
				yield break;
			}
			if (!this.tournamentCloudManager.TournamentEnded)
			{
				object vs2 = this.GameDialogs.ShowMessageBox(L._("Unexpected Time Travelling"), L._("Sorry, it looks like the time has jumped and the tournament had not actually ended!"), L._("Ok"), null);
				yield return this.GameDialogs.WaitForClosingView(vs2);
				yield break;
			}
		}
		yield return flowProvider.ShowTournamentEnded();
		if (this.Cloud.IsTournamentResultReady)
		{
			TournamentPrizeConfig prizeGiven;
			if (this.TryClaimRewardAndAddToInventory(out prizeGiven))
			{
				yield return flowProvider.ShowReward(prizeGiven);
			}
			else
			{
				object msgBox = this.GameDialogs.ShowMessageBox(L.Get("Sorry"), L.Get("You have already claimed the reward on another device."), L.Get("OK"), null);
				yield return this.GameDialogs.WaitForClosingView(msgBox);
			}
		}
		yield break;
	}

	private TournamentConfig Config
	{
		get
		{
			return ConfigurationManager.Get<TournamentConfig>();
		}
	}

	public LocalScore PersistedUnsubmittedScore
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("TournamentManagerUnsubmittedScore", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.HashtableToObject<LocalScore>(securedString.hashtableFromJson());
			}
			return null;
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("TournamentManagerUnsubmittedScore", JsonSerializer.ObjectToHashtable(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("TournamentManagerUnsubmittedScore", string.Empty);
			}
		}
	}

	private const string LIVES_TIME_STAMP = "TournamentManagerLivesTimeStamp";

	public const int AMOUNT_OF_LEVELS = 6;

	private TournamentCloudManager tournamentCloudManager;

	private TournamentManager.ITournamentUIProvider uiProvider;

	private RegeneratingItemService regeneratingTournamentLives;

	private LevelDatabaseCollection levelDatabaseCollection;

	private readonly ILivesManager livesManager;

	private const string PERSISTED_UNSUBMITTED_SCORE = "TournamentManagerUnsubmittedScore";

	public interface IFlowProvider
	{
		IEnumerator ShowTournamentEnded();

		IEnumerator JoinNewTournament();

		IEnumerator ShowReward(TournamentPrizeConfig reward);
	}

	public interface ITournamentUIProvider
	{
		void ScheduleNotificationWhenTournamentIsJoinable(DateTime utcEndTime);
	}

	public interface ICheckStateProvider
	{
		IEnumerator ShowTournamentEnded(Action<bool> result);
	}

	[SettingsProvider("tu", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<TournamentManager.PersistableState>, IPersistableState
	{
		public PersistableState()
		{
			this.TournamentUnlocked = 0;
		}

		[JsonSerializable("tu", null)]
		public int TournamentUnlocked { get; set; }

		public void MergeFromOther(TournamentManager.PersistableState newState, TournamentManager.PersistableState lastCloudState)
		{
			this.TournamentUnlocked = Math.Max(newState.TournamentUnlocked, this.TournamentUnlocked);
		}
	}
}
