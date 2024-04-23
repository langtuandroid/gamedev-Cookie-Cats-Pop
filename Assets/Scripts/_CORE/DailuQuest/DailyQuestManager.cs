using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tactile;
using UnityEngine;

public class DailyQuestManager : MapPopupManager.IMapPopup
{
	private DailyQuestManager(DailyQuestManager.IDailyQuestManagerInterface managerInterface, LevelDatabaseCollection pLevelDatabaseCollection)
	{
		this.managerInterface = managerInterface;
		this.managerInterface.RegisterUnlockPopup(this);
		this._levelDatabaseCollection = pLevelDatabaseCollection;
		this._levelDatabaseCollection.RegisterLevelDatabaseListener("Daily");
		if (DailyQuestManager.PersistableState.UpdateStructureToLatestVersionIfNeeded(this.State))
		{
			this.SaveState();
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnChallengeReset;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnQuestCompleted;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnPendingRewardConsumed;

	private bool NeedSync { get; set; }

	private DailyQuestManager.IUpdateDelegate UpdateDelegate { get; set; }

	public static DailyQuestManager Instance { get; private set; }

	public static DailyQuestManager CreateInstance(DailyQuestManager.IDailyQuestManagerInterface managerInterface, LevelDatabaseCollection pLevelDatabaseCollection)
	{
		if (DailyQuestManager.Instance != null)
		{
			return DailyQuestManager.Instance;
		}
		DailyQuestManager.Instance = new DailyQuestManager(managerInterface, pLevelDatabaseCollection);
		return DailyQuestManager.Instance;
	}

	private DailyQuestManager.PersistableState State
	{
		get
		{
			return this.managerInterface.PersistableState;
		}
	}

	private int MaxDailyQuests
	{
		get
		{
			return this.Config.Quests.Count;
		}
	}

	public DailyQuestConfig Config
	{
		get
		{
			return this.managerInterface.Config;
		}
	}

	private List<DailyQuestInfo> DailyQuestInfos
	{
		get
		{
			return this.managerInterface.DayItems;
		}
	}

	private bool HasPendingReward
	{
		get
		{
			return this.State.PendingReward != null;
		}
	}

	private int CurrentQuestIndex
	{
		get
		{
			return this.State.CompletedQuests.Count;
		}
	}

	private int FurthestAvailableQuestIndex
	{
		get
		{
			return this.MaxDailyQuests - this.State.MissedQuestCount - 1;
		}
	}

	private int UnrecordedMissedQuests
	{
		get
		{
			TimeSpan timeSpan = DateTime.Now - this.State.QuestTimerEnd;
			if (timeSpan.TotalSeconds <= 0.0)
			{
				return 0;
			}
			return (int)Mathf.Ceil((float)timeSpan.TotalHours / 24f);
		}
	}

	public LevelProxy CurrentQuestLevel
	{
		get
		{
			DailyQuestLevelDatabase levelDatabase = this._levelDatabaseCollection.GetLevelDatabase<DailyQuestLevelDatabase>("Daily");
			return levelDatabase.GetLevel(this.State.CompletedQuests.Count);
		}
	}

	public bool IsChallengeAvailable
	{
		get
		{
			return this.Config.InUse && this.managerInterface.FarthestUnlockedLevelHumanNumber >= this.managerInterface.Config.LevelRequiredForDailyQuests;
		}
	}

	public int DaysLeftInChallenge
	{
		get
		{
			if (this.State.ChallengeStartDateString == null)
			{
				return 0;
			}
			return this.MaxDailyQuests - DateHelper.DifferenceInDays(this.State.ChallengeStartDate, DateTime.Now, true);
		}
	}

	public bool HasQuestCooldown
	{
		get
		{
			return this.State.QuestCooldownEndDateString != null && this.GetSecondsLeftInCooldown() > 0;
		}
	}

	public DateTime CurrentQuestExpiryEnd
	{
		get
		{
			return this.State.QuestTimerEnd;
		}
	}

	public bool WantAttention
	{
		get
		{
			return this.IsChallengeAvailable && (this.HasPendingReward || !this.HasQuestCooldown);
		}
	}

	private DailyQuestChallengeState GetCurrentChallengeState()
	{
		DailyQuestChallengeState dailyQuestChallengeState = new DailyQuestChallengeState();
		dailyQuestChallengeState.CurrentQuestIndex = this.CurrentQuestIndex;
		dailyQuestChallengeState.DaysLeftInChallenge = this.DaysLeftInChallenge;
		dailyQuestChallengeState.HasPendingReward = this.HasPendingReward;
		dailyQuestChallengeState.HasQuestCooldown = this.HasQuestCooldown;
		List<DailyQuestManager.QuestState> list = new List<DailyQuestManager.QuestState>();
		for (int i = 0; i < this.MaxDailyQuests; i++)
		{
			list.Add(this.GetQuestStateForIndex(i));
		}
		dailyQuestChallengeState.QuestStates = list;
		dailyQuestChallengeState.FriendProgress = this.GetLastKnownFriendsProgress();
		return dailyQuestChallengeState;
	}

	private DailyQuestManager.QuestState GetQuestStateForIndex(int questIndex)
	{
		DailyQuestManager.QuestState result = DailyQuestManager.QuestState.Unknown;
		if (questIndex < this.State.CompletedQuests.Count)
		{
			result = DailyQuestManager.QuestState.Completed;
		}
		else if (questIndex < this.MaxDailyQuests - this.State.MissedQuestCount)
		{
			result = DailyQuestManager.QuestState.Incomplete;
		}
		else if (questIndex >= this.MaxDailyQuests - this.State.MissedQuestCount)
		{
			result = DailyQuestManager.QuestState.Missed;
		}
		return result;
	}

	public IEnumerator Checkin(DailyQuestManager.IUpdateDelegate updateDelegate)
	{
		this.UpdateDelegate = updateDelegate;
		if (!this.State.IsChallengeRunning)
		{
			this.StartChallenge();
		}
		this.UpdateDelegate.OnStateUpdated(this.GetCurrentChallengeState());
		if (this.State.GiveHeadStart)
		{
			yield return this.RunHeadStartFlow();
			this.State.GiveHeadStart = false;
			this.SaveState();
		}
		if (this.HasPendingReward)
		{
			yield return this.ConsumePendingRewardCr();
		}
		if (this.State.HasChallengeExpired)
		{
			yield return this.UpdateDelegate.OnChallengeExpired();
			this.ResetChallenge();
		}
		int unrecordedMissedQuests = this.UnrecordedMissedQuests;
		if (unrecordedMissedQuests != 0)
		{
			yield return this.UpdateDelegate.OnMissedQuests(unrecordedMissedQuests, this.FurthestAvailableQuestIndex);
			this.State.MissedQuestCount += unrecordedMissedQuests;
			string missedDate = DateTime.Now.ToString("g");
			for (int i = 0; i < unrecordedMissedQuests; i++)
			{
				this.State.MissedQuests.Add(new QuestStateInfo(missedDate, false, false));
			}
			this.State.QuestTimerEnd = DateTime.Now.Date.AddDays(1.0);
			this.State.QuestCooldownEndDateString = null;
			this.SaveState();
			if (this.State.HasChallengeExpired)
			{
				yield return this.UpdateDelegate.OnChallengeExpired();
				this.ResetChallenge();
				this.UpdateDelegate.OnStateUpdated(this.GetCurrentChallengeState());
			}
		}
		List<DailyQuestManager.FriendProgressionState> friendProgressionStates = this.GetFriendProgressionStates();
		yield return this.UpdateDelegate.OnFriendProgression(friendProgressionStates);
		this.MarkLastKnownFriendsProgress();
		yield break;
	}

	private IEnumerator RunHeadStartFlow()
	{
		if (this.CurrentQuestIndex >= this.Config.HeadStartAmountInDays)
		{
			yield break;
		}
		yield return this.UpdateDelegate.OnShowHeadStartView();
		UICamera.DisableInput();
		while (this.CurrentQuestIndex < this.Config.HeadStartAmountInDays)
		{
			this.State.PendingReward = new PendingReward(this.CurrentQuestIndex, true);
			yield return this.ConsumePendingRewardCr();
			this.UpdateDelegate.OnStateUpdated(this.GetCurrentChallengeState());
		}
		UICamera.EnableInput();
		yield break;
	}

	public void Checkout()
	{
		this.UpdateDelegate = null;
		if (this.NeedSync)
		{
			this.NeedSync = false;
			this.managerInterface.SyncUserSettings();
		}
	}

	public void RequestCurrentState()
	{
		if (this.UpdateDelegate != null)
		{
			this.UpdateDelegate.OnStateUpdated(this.GetCurrentChallengeState());
		}
	}

	public string GetCurrentStateString()
	{
		if (!this.IsChallengeAvailable)
		{
			return "No Challenge Available";
		}
		DailyQuestManager.PersistableState state = this.State;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("MaxDailyQuests: " + state.MaxDailyQuests);
		stringBuilder.AppendLine("ChallengeStartDate: " + state.ChallengeStartDateString);
		stringBuilder.AppendLine("QuestTimerEnd: " + state.QuestTimerEndDateString);
		stringBuilder.AppendLine("QuestCooldownEnd: " + state.QuestCooldownEndDateString);
		stringBuilder.AppendLine("Missed Quest Count: " + state.MissedQuestCount);
		stringBuilder.AppendLine("Pending Reward: " + ((!(state.PendingReward != null)) ? "none" : state.PendingReward.ToString()));
		if (state.CompletedQuests.Count == 0)
		{
			stringBuilder.AppendLine("No completed quests");
		}
		else
		{
			stringBuilder.AppendLine("Completed Quests (" + state.CompletedQuests.Count + "):");
			foreach (QuestStateInfo questStateInfo in state.CompletedQuests)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"Quest: ",
					questStateInfo.DateString,
					" skipped: ",
					questStateInfo.Skipped
				}));
			}
		}
		if (state.MissedQuests.Count == 0)
		{
			stringBuilder.AppendLine("No missed quests");
		}
		else
		{
			stringBuilder.AppendLine("Missed Quests (" + state.MissedQuests.Count + "):");
			foreach (QuestStateInfo questStateInfo2 in state.MissedQuests)
			{
				stringBuilder.AppendLine("Quest: " + questStateInfo2.DateString + " ");
			}
		}
		if (state.SkippedCooldowns.Count == 0)
		{
			stringBuilder.AppendLine("No skipped cooldowns");
		}
		else
		{
			stringBuilder.AppendLine("Skipped Cooldowns (" + state.SkippedCooldowns.Count + "):");
			foreach (string str in state.SkippedCooldowns)
			{
				stringBuilder.AppendLine("Skipped on: " + str + " ");
			}
		}
		return stringBuilder.ToString();
	}

	private void ResetChallenge()
	{
		this.State.Reset(this.Config.Quests.Count);
		this.SaveState();
		if (this.OnChallengeReset != null)
		{
			this.OnChallengeReset();
		}
	}

	public void StartChallenge()
	{
		if (!this.State.IsChallengeRunning && this.IsChallengeAvailable)
		{
			this.ResetChallenge();
		}
	}

	public void SubmitCurrentDailyQuestCompletion()
	{
		this.State.PendingReward = new PendingReward(this.CurrentQuestIndex, false);
		this.State.QuestTimerEnd = DateTime.Now.AddDays(1.0).Date.AddDays(1.0);
		this.State.QuestCooldownEnd = DateTime.Now.Date.AddDays(1.0);
		this.managerInterface.SaveLocalSettings();
		this.managerInterface.SyncUserSettings();
		if (this.OnQuestCompleted != null)
		{
			this.OnQuestCompleted();
		}
	}

	public void SkipCooldown()
	{
		this.State.SkippedCooldowns.Add(DateTime.Now.ToString("g"));
		this.State.QuestCooldownEndDateString = null;
		this.State.QuestTimerEnd = DateTime.Now.AddDays(1.0).Date;
		this.State.MissedQuestCount = Mathf.Max(this.State.MissedQuestCount - 1, 0);
		this.SaveState();
	}

	public void SkipCurrentQuest()
	{
		this.State.PendingReward = new PendingReward(this.CurrentQuestIndex, true);
		this.State.QuestTimerEnd = DateTime.Now.AddDays(1.0).Date;
		this.State.MissedQuestCount = Mathf.Max(this.State.MissedQuestCount - 1, 0);
		this.SaveState();
		if (this.OnQuestCompleted != null)
		{
			this.OnQuestCompleted();
		}
	}

	private void SaveState()
	{
		this.NeedSync = true;
		this.managerInterface.SaveLocalSettings();
		if (this.UpdateDelegate != null)
		{
			this.UpdateDelegate.OnStateUpdated(this.GetCurrentChallengeState());
		}
	}

	public DailyQuestInfo GetQuestInfo(int index)
	{
		if (index < 0 || index >= this.DailyQuestInfos.Count)
		{
			return null;
		}
		return this.DailyQuestInfos[index];
	}

	public bool ManualConsumePendingReward()
	{
		if (!this.HasPendingReward)
		{
			return false;
		}
		FiberCtrl.Pool.Run(this.ManualConsumePendingRewardCr(), false);
		return true;
	}

	private IEnumerator ManualConsumePendingRewardCr()
	{
		UICamera.DisableInput();
		if (this.HasPendingReward)
		{
			yield return this.ConsumePendingRewardCr();
		}
		if (this.State.HasChallengeExpired)
		{
			yield return this.UpdateDelegate.OnChallengeExpired();
			this.ResetChallenge();
		}
		UICamera.EnableInput();
		yield break;
	}

	private IEnumerator ConsumePendingRewardCr()
	{
		bool isLastReward = this.State.CompletedQuests.Count + 1 >= this.State.MaxDailyQuests - this.State.MissedQuestCount;
		int questIndex = this.State.PendingReward.QuestIndex;
		yield return this.UpdateDelegate.OnConsumeReward(questIndex, this.Config.Quests[questIndex], isLastReward);
		this.managerInterface.SavePublicCompletedQuestIndex(this.State.CompletedQuests.Count + 1);
		string completionDate = DateTime.Now.ToString("g");
		this.State.CompletedQuests.Add(new QuestStateInfo(completionDate, this.State.PendingReward.Skipped, true));
		this.State.PendingReward = null;
		this.SaveState();
		if (this.OnPendingRewardConsumed != null)
		{
			this.OnPendingRewardConsumed();
		}
		yield break;
	}

	private bool ShouldShowPopup(int unlockedLevelIndex)
	{
		return this.IsChallengeAvailable && this.Config.LevelRequiredForDailyQuests == unlockedLevelIndex + 1;
	}

	private bool ShouldRunSilentAction()
	{
		return this.IsChallengeAvailable && !this.State.IsChallengeRunning;
	}

	void MapPopupManager.IMapPopup.TryShowPopup(int unlockedLevelIndex, MapPopupManager.PopupFlow popupFlow)
	{
		if (this.ShouldShowPopup(unlockedLevelIndex))
		{
			if (!this.State.IsChallengeRunning)
			{
				if (this.Config.HeadStartAmountInDays > 0)
				{
					this.State.GiveHeadStart = true;
				}
				this.StartChallenge();
			}
			popupFlow.AddPopup(this.managerInterface.ShowUnlockPopup());
		}
		else if (this.ShouldRunSilentAction())
		{
			popupFlow.AddSilentAction(new Action(this.StartChallenge));
		}
	}

	private int GetSecondsLeftTillQuestExpired()
	{
		if (!this.State.IsChallengeRunning)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		DateTime questTimerEnd = this.State.QuestTimerEnd;
		return (int)(questTimerEnd - now).TotalSeconds;
	}

	private int GetSecondsLeftInCooldown()
	{
		if (this.State.QuestCooldownEndDateString == null)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		DateTime questCooldownEnd = this.State.QuestCooldownEnd;
		return (int)(questCooldownEnd - now).TotalSeconds;
	}

	public string GetSecondsLeftTillQuestExpiredStr()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.GetSecondsLeftTillQuestExpired());
		if (timeSpan.TotalSeconds <= 0.0)
		{
			return "00:00:00";
		}
		return string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public string GetSecondsLeftInCooldownStr()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.GetSecondsLeftInCooldown());
		return string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public Dictionary<string, int> GetLastKnownFriendsProgress()
	{
		List<CloudUser> cachedFriends = this.managerInterface.CachedFriends;
		if (cachedFriends == null)
		{
			return null;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, int> persistedLastKnownFriendsProgress = this.PersistedLastKnownFriendsProgress;
		foreach (CloudUser cloudUser in cachedFriends)
		{
			if (!persistedLastKnownFriendsProgress.ContainsKey(cloudUser.FacebookId))
			{
				dictionary[cloudUser.FacebookId] = 0;
			}
			else
			{
				dictionary[cloudUser.FacebookId] = persistedLastKnownFriendsProgress[cloudUser.FacebookId];
			}
		}
		return dictionary;
	}

	private Dictionary<string, int> GetLatestFriendsProgress()
	{
		List<CloudUser> cachedFriends = this.managerInterface.CachedFriends;
		if (cachedFriends == null)
		{
			return null;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (CloudUser cloudUser in cachedFriends)
		{
			dictionary.Add(cloudUser.FacebookId, this.managerInterface.CompletedQuestIndexForFriend(cloudUser));
		}
		return dictionary;
	}

	private void MarkLastKnownFriendsProgress()
	{
		this.PersistedLastKnownFriendsProgress = this.GetLatestFriendsProgress();
	}

	private int GetLastKnownProgressForFriend(string facebookId)
	{
		int result;
		this.PersistedLastKnownFriendsProgress.TryGetValue(facebookId, out result);
		return result;
	}

	private List<DailyQuestManager.FriendProgressionState> GetFriendProgressionStates()
	{
		List<DailyQuestManager.FriendProgressionState> list = new List<DailyQuestManager.FriendProgressionState>();
		List<CloudUser> cachedFriends = this.managerInterface.CachedFriends;
		foreach (CloudUser cloudUser in cachedFriends)
		{
			int lastKnownProgressForFriend = this.GetLastKnownProgressForFriend(cloudUser.FacebookId);
			int num = this.managerInterface.CompletedQuestIndexForFriend(cloudUser);
			if (num > lastKnownProgressForFriend)
			{
				list.Add(new DailyQuestManager.FriendProgressionState
				{
					FacebookId = cloudUser.FacebookId,
					OldQuestIndex = lastKnownProgressForFriend,
					NewQuestIndex = num
				});
			}
		}
		return list;
	}

	private Dictionary<string, int> PersistedLastKnownFriendsProgress
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("PersistedFriendsCompletedQuest", string.Empty);
			if (securedString.Length > 0)
			{
				Dictionary<string, int> progressPerFriend = JsonSerializer.HashtableToObject<DailyQuestManager.FriendsProgression>(securedString.hashtableFromJson()).ProgressPerFriend;
				if (progressPerFriend != null)
				{
					return progressPerFriend;
				}
			}
			return new Dictionary<string, int>();
		}
		set
		{
			string value2 = string.Empty;
			if (value != null)
			{
				value2 = JsonSerializer.ObjectToHashtable(new DailyQuestManager.FriendsProgression
				{
					ProgressPerFriend = value
				}).toJson();
			}
			try
			{
				TactilePlayerPrefs.SetSecuredString("PersistedFriendsCompletedQuest", value2);
			}
			catch (Exception ex)
			{
			}
		}
	}

	private const string PREFS_FRIENDS_COMPLETED_QUEST = "PersistedFriendsCompletedQuest";

	private DailyQuestManager.IDailyQuestManagerInterface managerInterface;

	private LevelDatabaseCollection _levelDatabaseCollection;

	public enum QuestState
	{
		Unknown,
		Completed,
		Incomplete,
		Missed
	}

	public class FriendProgressionState
	{
		public int OldQuestIndex { get; set; }

		public int NewQuestIndex { get; set; }

		public string FacebookId { get; set; }
	}

	public class FriendsProgression
	{
		[JsonSerializable("dqfo", typeof(int))]
		public Dictionary<string, int> ProgressPerFriend { get; set; }
	}

	[SettingsProvider("dq", false, new Type[]
	{

	})]
	public class PersistableState : IPersistableState<DailyQuestManager.PersistableState>, IPersistableState
	{
		public PersistableState()
		{
			this.CompletedQuests = new List<QuestStateInfo>();
			this.MissedQuests = new List<QuestStateInfo>();
			this.SkippedCooldowns = new List<string>();
			this.ChallengeStartDateString = null;
			this.QuestTimerEndDateString = null;
			this.QuestCooldownEndDateString = null;
			this.PendingReward = null;
			this.MaxDailyQuests = 0;
			this.MissedQuestCount = 0;
		}

		[JsonSerializable("lcq", null)]
		public int LastCompletedQuestDayId { get; set; }

		[JsonSerializable("cqn", null)]
		public int CompletedQuestNumber { get; set; }

		[JsonSerializable("pr", null)]
		public bool OldPendingReward { get; set; }

		[JsonSerializable("sd", null)]
		public int StartDayId { get; set; }

		[JsonSerializable("lst", null)]
		public int LastNumberOfUnavailableQuests { get; set; }

		[JsonSerializable("npr", null)]
		public PendingReward PendingReward { get; set; }

		[JsonSerializable("sds", null)]
		public string ChallengeStartDateString { get; set; }

		[JsonSerializable("qte", null)]
		public string QuestTimerEndDateString { get; set; }

		[JsonSerializable("qce", null)]
		public string QuestCooldownEndDateString { get; set; }

		[JsonSerializable("cq", typeof(QuestStateInfo))]
		public List<QuestStateInfo> CompletedQuests { get; set; }

		[JsonSerializable("mq", null)]
		public int MissedQuestCount { get; set; }

		[JsonSerializable("mql", typeof(QuestStateInfo))]
		public List<QuestStateInfo> MissedQuests { get; set; }

		[JsonSerializable("scd", typeof(string))]
		public List<string> SkippedCooldowns { get; set; }

		[JsonSerializable("mdq", null)]
		public int MaxDailyQuests { get; set; }

		[JsonSerializable("ghs", null)]
		public bool GiveHeadStart { get; set; }

		public DateTime ChallengeStartDate
		{
			get
			{
				return DateHelper.GetDateTimeFromString(this.ChallengeStartDateString, "g");
			}
			set
			{
				this.ChallengeStartDateString = value.ToString("g");
			}
		}

		public DateTime QuestTimerEnd
		{
			get
			{
				return DateHelper.GetDateTimeFromString(this.QuestTimerEndDateString, "g");
			}
			set
			{
				this.QuestTimerEndDateString = value.ToString("g");
			}
		}

		public DateTime QuestCooldownEnd
		{
			get
			{
				return DateHelper.GetDateTimeFromString(this.QuestCooldownEndDateString, "g");
			}
			set
			{
				this.QuestCooldownEndDateString = value.ToString("g");
			}
		}

		public bool HasChallengeExpired
		{
			get
			{
				bool flag = DateHelper.DifferenceInDays(this.ChallengeStartDate, DateTime.Now, true) >= this.MaxDailyQuests;
				bool flag2 = this.CompletedQuests.Count >= this.MaxDailyQuests - this.MissedQuestCount;
				return flag || flag2;
			}
		}

		public bool IsChallengeRunning
		{
			get
			{
				return this.ChallengeStartDateString != null;
			}
		}

		public void Reset(int maxDailyQuests)
		{
			this.CompletedQuests = new List<QuestStateInfo>();
			this.MissedQuests = new List<QuestStateInfo>();
			this.SkippedCooldowns = new List<string>();
			this.ChallengeStartDate = DateTime.Now;
			this.QuestTimerEnd = DateTime.Now.Date.AddDays(1.0);
			this.QuestCooldownEndDateString = null;
			this.PendingReward = null;
			this.MaxDailyQuests = maxDailyQuests;
			this.MissedQuestCount = 0;
		}

		public void MergeFromOther(DailyQuestManager.PersistableState newest, DailyQuestManager.PersistableState last)
		{
			this.MergeOtherIntoThis(newest, UserSettingsManager.Get<DailyQuestManager.PersistableState>());
		}

		internal static bool UpdateStructureToLatestVersionIfNeeded(DailyQuestManager.PersistableState state)
		{
			if (state.StartDayId != 0)
			{
				if (state.OldPendingReward)
				{
					state.PendingReward = new PendingReward(state.CompletedQuestNumber, false);
				}
				state.ChallengeStartDate = DateHelper.DateTimeFromDayId(state.StartDayId);
				state.MissedQuestCount = state.LastNumberOfUnavailableQuests;
				state.QuestTimerEnd = DateTime.UtcNow.Date.AddDays(1.0);
				state.MaxDailyQuests = 24;
				List<QuestStateInfo> list = new List<QuestStateInfo>();
				for (int i = 0; i < state.CompletedQuestNumber; i++)
				{
					list.Add(new QuestStateInfo
					{
						Date = DateHelper.DefaultTime
					});
				}
				state.CompletedQuests = list;
				state.LastCompletedQuestDayId = 0;
				state.CompletedQuestNumber = 0;
				state.OldPendingReward = false;
				state.StartDayId = 0;
				state.LastNumberOfUnavailableQuests = 0;
				return true;
			}
			return false;
		}

		public void MergeOtherIntoThis(DailyQuestManager.PersistableState other, DailyQuestManager.PersistableState last)
		{
			DailyQuestManager.PersistableState.UpdateStructureToLatestVersionIfNeeded(other);
			if (!other.HasChallengeExpired && (other.CompletedQuests.Count > this.CompletedQuests.Count || this.HasChallengeExpired))
			{
				this.PendingReward = other.PendingReward;
				this.ChallengeStartDateString = other.ChallengeStartDateString;
				this.QuestTimerEndDateString = other.QuestTimerEndDateString;
				this.QuestCooldownEndDateString = other.QuestCooldownEndDateString;
				this.CompletedQuests = other.CompletedQuests;
				this.MissedQuests = other.MissedQuests;
				this.MissedQuestCount = other.MissedQuestCount;
				this.MaxDailyQuests = other.MaxDailyQuests;
				this.SkippedCooldowns = other.SkippedCooldowns;
			}
		}
	}

	public interface IDailyQuestManagerInterface : IPlayerState
	{
		void RegisterUnlockPopup(DailyQuestManager manager);

		IEnumerator ShowUnlockPopup();

		DailyQuestManager.PersistableState PersistableState { get; }

		List<CloudUser> CachedFriends { get; }

		DailyQuestConfig Config { get; }

		int CompletedQuestIndexForFriend(CloudUser friend);

		CloudUser GetCloudUserForFacebookId(string facebookId);

		void SavePublicCompletedQuestIndex(int index);

		List<DailyQuestInfo> DayItems { get; }

		void SyncUserSettings();

		void SaveLocalSettings();
	}

	public interface IUpdateDelegate
	{
		IEnumerator OnConsumeReward(int questIndex, DailyQuestInfo info, bool isLastReward);

		IEnumerator OnShowHeadStartView();

		IEnumerator OnChallengeExpired();

		IEnumerator OnMissedQuests(int missed, int furthestAvailableQuestIndex);

		IEnumerator OnFriendProgression(List<DailyQuestManager.FriendProgressionState> states);

		void OnStateUpdated(DailyQuestChallengeState state);
	}

	public class DailyQuestCommandHandler : BaseCommandHandler
	{
		
		private static void CompleteQuests(int count = 1)
		{
			string dateString = DateTime.Now.ToString("g");
			for (int i = 0; i < count; i++)
			{
				DailyQuestManager.Instance.State.CompletedQuests.Add(new QuestStateInfo(dateString, false, true));
			}
			DailyQuestManager.Instance.SaveState();
		}

		
		private static void MissQuests(int amount = 1)
		{
			DailyQuestManager.Instance.State.MissedQuestCount += amount;
			DailyQuestManager.Instance.SaveState();
		}

		
		private static void ResetChallenge()
		{
			DailyQuestManager.Instance.ResetChallenge();
		}

		
		private static void PrintCurrentState()
		{
		}
	}

	[SettingsProvider("dqPublic", true, new Type[]
	{

	})]
	public class PublicState : IPersistableState<DailyQuestManager.PublicState>, IPersistableState
	{
		[JsonSerializable("ldq", null)]
		public int CompletedQuestNumber { get; set; }

		public void MergeFromOther(DailyQuestManager.PublicState otherState, DailyQuestManager.PublicState lastCloudState)
		{
			this.CompletedQuestNumber = Mathf.Max(this.CompletedQuestNumber, otherState.CompletedQuestNumber);
		}
	}
}
