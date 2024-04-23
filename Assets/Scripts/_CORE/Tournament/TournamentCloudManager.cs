using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CollectionUtils;
using Fibers;

public class TournamentCloudManager
{
	public TournamentCloudManager(CloudClient cloudClient)
	{
		this.cloudClient = cloudClient;
		this.state = this.PersistedState;
		this.UpdateSortedScores();
		FiberCtrl.Pool.Run(this.TournamentEndedChecker(), false);
	}

	private IEnumerator TournamentEndedChecker()
	{
		bool wasEnded = false;
		for (;;)
		{
			if (!wasEnded && this.TournamentEnded)
			{
				this.OnTournamentEndedEvent();
			}
			wasEnded = this.TournamentEnded;
			yield return new Fiber.Wait(1f);
		}
		yield break;
	}

	private void UpdateTournamentState(List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int endsInSeconds, int closesInSeconds, DateTime utcReceived, TournamentCloudManager.PresentResult presentResult = TournamentCloudManager.PresentResult.Unknown)
	{
		if (!this.TournamentJoined || this.state.PresentResult == 1 || this.state.PresentResult == 2)
		{
		}
		this.state = new TournamentCloudManager.State();
		this.state.Entries = entries;
		this.state.Users = users;
		this.state.PeriodId = periodId;
		this.state.TournamentId = tournamentId;
		this.state.EndsInSeconds = endsInSeconds;
		this.state.ClosesInSeconds = closesInSeconds;
		this.state.UtcReceived = utcReceived;
		this.state.PresentResult = (int)presentResult;
		this.PersistedState = this.state;
		this.UpdateSortedScores();
		this.OnTournamentStateUpdatedEvent();
	}

	private void UpdateSortedScores()
	{
		string localDeviceId = (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
		string localUserId = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
		this.sortedScores = new Dictionary<int, List<TournamentCloudManager.Score>>(IntComparer.Instance);
		this.sortedOverallScores = new List<TournamentCloudManager.Score>();
		foreach (TournamentCloudManager.Entry entry in this.state.Entries)
		{
			CloudUser user = this.state.GetUser(entry.UserId);
			TournamentCloudManager.Score score = new TournamentCloudManager.Score
			{
				score = 0,
				deviceId = entry.DeviceId,
				userId = entry.UserId,
				facebookId = ((user == null) ? null : user.ExternalId),
				displayName = ((user == null) ? null : user.DisplayName)
			};
			this.sortedOverallScores.Add(score);
			foreach (KeyValuePair<string, int> keyValuePair in entry.Scores)
			{
				int key = 0;
				if (int.TryParse(keyValuePair.Key, out key))
				{
					if (!this.sortedScores.ContainsKey(key))
					{
						this.sortedScores[key] = new List<TournamentCloudManager.Score>();
					}
					score.score += keyValuePair.Value;
					this.sortedScores[key].Add(new TournamentCloudManager.Score
					{
						score = keyValuePair.Value,
						deviceId = entry.DeviceId,
						userId = entry.UserId,
						facebookId = ((user == null) ? null : user.ExternalId),
						displayName = ((user == null) ? null : user.DisplayName)
					});
				}
			}
		}
		foreach (KeyValuePair<int, List<TournamentCloudManager.Score>> keyValuePair2 in this.sortedScores)
		{
			keyValuePair2.Value.Sort((TournamentCloudManager.Score x, TournamentCloudManager.Score y) => y.CompareTo(x, localDeviceId, localUserId));
			for (int i = 0; i < keyValuePair2.Value.Count; i++)
			{
				keyValuePair2.Value[i].position = i + 1;
			}
		}
		this.sortedOverallScores.Sort((TournamentCloudManager.Score x, TournamentCloudManager.Score y) => y.CompareTo(x, localDeviceId, localUserId));
		for (int j = 0; j < this.sortedOverallScores.Count; j++)
		{
			this.sortedOverallScores[j].position = j + 1;
		}
	}

	public IEnumerator UpdateStatus(Action<object> callback)
	{
		if (this.TournamentJoined)
		{
			callback("User already in a tournament - unable to update status until current one is cleared");
			yield break;
		}
		object error = null;
		yield return this.cloudClient.TournamentStatus(delegate(object err, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int tournamentEndInSeconds, int tournamentClosesInSeconds, DateTime utcReceived)
		{
			error = err;
			if (err == null)
			{
				this.UpdateTournamentState(entries, users, periodId, tournamentId, tournamentEndInSeconds, tournamentClosesInSeconds, utcReceived, TournamentCloudManager.PresentResult.Unknown);
			}
		});
		callback(error);
		yield break;
	}

	public IEnumerator Join(TournamentCloudManager.Type type, Action<object> callback)
	{
		if (this.TournamentJoined)
		{
			callback("User already in a tournament - unable to join another tournament until current one is cleared");
			yield break;
		}
		object error = null;
		yield return this.cloudClient.TournamentJoin(type, delegate(object err, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int tournamentEndInSeconds, int tournamentClosesInSeconds, DateTime utcReceived)
		{
			error = err;
			if (err == null)
			{
				this.UpdateTournamentState(entries, users, periodId, tournamentId, tournamentEndInSeconds, tournamentClosesInSeconds, utcReceived, TournamentCloudManager.PresentResult.Unknown);
			}
		});
		callback(error);
		yield break;
	}

	public IEnumerator SubmitScore(int leaderboard, int score, Action<object> callback)
	{
		if (!this.TournamentJoined)
		{
			callback(null);
			yield break;
		}
		int tournamentScore = this.state.GetScore(leaderboard, (!this.cloudClient.HasValidDevice) ? null : this.cloudClient.CachedDevice.CloudId, (!this.cloudClient.HasValidUser) ? null : this.cloudClient.CachedMe.CloudId);
		object error = null;
		if (score > tournamentScore)
		{
			yield return this.cloudClient.TournamentSubmitScore(this.state.PeriodId, this.state.TournamentId, leaderboard, score, delegate(object err, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int tournamentEndInSeconds, int tournamentClosesInSeconds, DateTime utcReceived)
			{
				error = err;
				if (err == null)
				{
					this.UpdateTournamentState(entries, users, periodId, tournamentId, tournamentEndInSeconds, tournamentClosesInSeconds, utcReceived, TournamentCloudManager.PresentResult.Unknown);
				}
			});
		}
		callback(error);
		yield break;
	}

	public IEnumerator Update()
	{
		if (!this.TournamentJoined)
		{
			yield break;
		}
		if (!this.TournamentEnded)
		{
			yield return this.UpdateState();
		}
		else if (!this.IsTournamentResultReady)
		{
			yield return this.UpdateResult();
		}
		yield break;
	}

	private IEnumerator UpdateState()
	{
		if (this.updateStateInProgress)
		{
			yield break;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.updateStateInProgress = false;
		});
		this.updateStateInProgress = true;
		if (!this.TournamentJoined)
		{
			yield break;
		}
		object error;
		yield return this.cloudClient.TournamentGetEntries(this.state.PeriodId, this.state.TournamentId, delegate(object err, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int tournamentEndInSeconds, int tournamentClosesInSeconds, DateTime utcReceived)
		{
			error = err;
			if (err == null)
			{
				this.UpdateTournamentState(entries, users, periodId, tournamentId, tournamentEndInSeconds, tournamentClosesInSeconds, utcReceived, TournamentCloudManager.PresentResult.Unknown);
			}
		});
		yield break;
	}

	private IEnumerator UpdateResult()
	{
		if (this.updateResultInProgress)
		{
			yield break;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.updateResultInProgress = false;
		});
		this.updateResultInProgress = true;
		if (!this.TournamentJoined)
		{
			yield break;
		}
		if (!this.TournamentEnded)
		{
			yield break;
		}
		if (this.IsTournamentResultReady)
		{
			yield break;
		}
		object error;
		yield return this.cloudClient.TournamentPresent(this.state.PeriodId, this.state.TournamentId, delegate(object err, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int tournamentEndInSeconds, int tournamentClosesInSeconds, DateTime utcReceived, TournamentCloudManager.PresentResult presentResult)
		{
			error = err;
			if (err == null)
			{
				if (presentResult == TournamentCloudManager.PresentResult.NoValidEntry)
				{
					this.Reset();
				}
				else
				{
					this.UpdateTournamentState(entries, users, periodId, tournamentId, tournamentEndInSeconds, tournamentClosesInSeconds, utcReceived, presentResult);
				}
			}
		});
		yield break;
	}

	public void Reset()
	{
		this.state = null;
		this.PersistedState = this.state;
	}

	public bool TournamentValid
	{
		get
		{
			return this.state != null && this.state.IsValid;
		}
	}

	public bool TournamentJoined
	{
		get
		{
			return this.TournamentValid && this.state.IsJoined;
		}
	}

	public bool TournamentOpen
	{
		get
		{
			return this.TournamentValid && DateTime.UtcNow <= this.state.UtcCloseTime;
		}
	}

	public bool TournamentEnded
	{
		get
		{
			return this.TournamentValid && DateTime.UtcNow > this.state.UtcEndTime;
		}
	}

	public bool TournamentActive
	{
		get
		{
			return this.TournamentJoined && DateTime.UtcNow <= this.state.UtcEndTime;
		}
	}

	public bool IsTournamentResultReady
	{
		get
		{
			return this.TournamentJoined && this.state.IsFinalResult && (this.state.PresentResult == 1 || this.state.PresentResult == 2);
		}
	}

	public bool TournamentResultPreviouslyPresented
	{
		get
		{
			return this.TournamentJoined && this.state.PresentResult == 2;
		}
	}

	public TournamentCloudManager.Type TournamentType
	{
		get
		{
			return (!this.TournamentJoined) ? TournamentCloudManager.Type.Unknown : this.state.TournamentType;
		}
	}

	public DateTime TournamentUtcEndTime
	{
		get
		{
			return (!this.TournamentValid) ? DateTime.UtcNow : this.state.UtcEndTime;
		}
	}

	public DateTime TournamentUtcCloseTime
	{
		get
		{
			return (!this.TournamentValid) ? DateTime.UtcNow : this.state.UtcCloseTime;
		}
	}

	public int TournamentId
	{
		get
		{
			return (!this.TournamentValid) ? -1 : this.state.TournamentId;
		}
	}

	public int PeriodId
	{
		get
		{
			return (!this.TournamentValid) ? -1 : this.state.PeriodId;
		}
	}

	public TournamentRank GetJoinedRank
	{
		get
		{
			if (this.TournamentId > -1)
			{
				return (TournamentRank)int.Parse(this.TournamentId.ToString()[0].ToString());
			}
			return TournamentRank.None;
		}
	}

	public List<TournamentCloudManager.Score> GetSortedLeaderboardScores(int leaderboard)
	{
		if (this.sortedScores.ContainsKey(leaderboard))
		{
			return this.sortedScores[leaderboard];
		}
		return new List<TournamentCloudManager.Score>();
	}

	public List<TournamentCloudManager.Score> SortedOverallScores
	{
		get
		{
			return this.sortedOverallScores;
		}
	}

	public TournamentCloudManager.Score GetMyLeaderboardScore(int leaderboard)
	{
		string deviceId = (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
		string userId = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
		int num = 0;
		if (this.sortedScores.ContainsKey(leaderboard))
		{
			num = this.sortedScores[leaderboard].Count;
			foreach (TournamentCloudManager.Score score in this.sortedScores[leaderboard])
			{
				if (score.IsOwnedByDeviceOrUser(deviceId, userId))
				{
					return score;
				}
			}
		}
		return new TournamentCloudManager.Score
		{
			score = 0,
			position = num + 1,
			deviceId = deviceId,
			userId = userId,
			facebookId = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.ExternalId),
			displayName = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.DisplayName)
		};
	}

	public TournamentCloudManager.Score GetMyOverallScore()
	{
		string deviceId = (!this.cloudClient.HasValidDevice) ? string.Empty : this.cloudClient.CachedDevice.CloudId;
		string userId = (!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.CloudId;
		foreach (TournamentCloudManager.Score score in this.sortedOverallScores)
		{
			if (score.IsOwnedByDeviceOrUser(deviceId, userId))
			{
				return score;
			}
		}
		return new TournamentCloudManager.Score
		{
			score = 0,
			position = this.sortedOverallScores.Count + 1,
			deviceId = deviceId,
			userId = userId,
			facebookId = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.ExternalId),
			displayName = ((!this.cloudClient.HasValidUser) ? string.Empty : this.cloudClient.CachedMe.DisplayName)
		};
	}

	private void OnTournamentStateUpdatedEvent()
	{
		if (this.TournamentStateUpdatedEvent != null)
		{
			this.TournamentStateUpdatedEvent();
		}
	}

	private void OnTournamentEndedEvent()
	{
		if (this.TournamentEndedEvent != null)
		{
			this.TournamentEndedEvent();
		}
	}

	private TournamentCloudManager.State PersistedState
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("TournamentCloudManagerState", string.Empty);
			TournamentCloudManager.State result = new TournamentCloudManager.State();
			if (securedString.Length > 0)
			{
				result = JsonSerializer.HashtableToObject<TournamentCloudManager.State>(securedString.hashtableFromJson());
			}
			return result;
		}
		set
		{
			if (value != null)
			{
				string value2 = JsonSerializer.ObjectToHashtable(this.state).toJson();
				TactilePlayerPrefs.SetSecuredString("TournamentCloudManagerState", value2);
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("TournamentCloudManagerState", string.Empty);
			}
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action TournamentStateUpdatedEvent;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action TournamentEndedEvent;

	private bool updateStateInProgress;

	private bool updateResultInProgress;

	private const string PREFS_STATE = "TournamentCloudManagerState";

	private TournamentCloudManager.State state;

	private Dictionary<int, List<TournamentCloudManager.Score>> sortedScores;

	private List<TournamentCloudManager.Score> sortedOverallScores;

	private CloudClient cloudClient;

	public enum Type
	{
		Unknown,
		Bronze,
		Silver,
		Gold
	}

	public enum PresentResult
	{
		Unknown = -2,
		NoValidEntry,
		TournamentActive,
		Presented,
		PreviouslyPresented
	}

	public class Entry
	{
		[JsonSerializable("userId", null)]
		public string UserId { get; set; }

		[JsonSerializable("deviceId", null)]
		public string DeviceId { get; set; }

		[JsonSerializable("scores", typeof(int))]
		public Dictionary<string, int> Scores { get; set; }

		public bool IsOwnedByDeviceOrUser(string deviceId, string userId)
		{
			return (!string.IsNullOrEmpty(this.UserId) && this.UserId == userId) || (!string.IsNullOrEmpty(this.DeviceId) && this.DeviceId == deviceId);
		}
	}

	public class Score
	{
		public Score()
		{
			this.submittedToCloud = false;
		}

		public Score(TournamentCloudManager.Score s)
		{
			this.score = s.score;
			this.position = s.position;
			this.deviceId = s.deviceId;
			this.userId = s.userId;
			this.facebookId = s.facebookId;
			this.displayName = s.displayName;
			this.submittedToCloud = s.submittedToCloud;
		}

		public string FirstName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.displayName))
				{
					string[] array = this.displayName.Split(new char[]
					{
						' '
					});
					if (array.Length > 0)
					{
						return array[0];
					}
				}
				return string.Empty;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"score=",
				this.score,
				", position=",
				this.position,
				", deviceId=",
				this.deviceId,
				", userId=",
				this.userId,
				", facebookId=",
				this.facebookId,
				", displayName=",
				this.displayName,
				", submittedToCloud=",
				this.submittedToCloud
			});
		}

		public bool IsOwnedByDeviceOrUser(string deviceId, string userId)
		{
			return (!string.IsNullOrEmpty(this.userId) && this.userId == userId) || (!string.IsNullOrEmpty(this.deviceId) && this.deviceId == deviceId);
		}

		public int CompareTo(TournamentCloudManager.Score b, string localDeviceId, string localUserId)
		{
			if (this == b)
			{
				return 0;
			}
			if (this.score != b.score)
			{
				return this.score.CompareTo(b.score);
			}
			if (this.IsOwnedByDeviceOrUser(localDeviceId, localUserId))
			{
				return 1;
			}
			if (b.IsOwnedByDeviceOrUser(localDeviceId, localUserId))
			{
				return -1;
			}
			return this.deviceId.CompareTo(b.deviceId);
		}

		public int score;

		public int position;

		public string deviceId;

		public string userId;

		public string facebookId;

		public string displayName;

		public bool submittedToCloud;
	}

	public class State
	{
		public State()
		{
			this.Entries = new List<TournamentCloudManager.Entry>();
		}

		[JsonSerializable("pid", null)]
		public int PeriodId { get; set; }

		[JsonSerializable("tid", null)]
		public int TournamentId { get; set; }

		[JsonSerializable("en", typeof(TournamentCloudManager.Entry))]
		public List<TournamentCloudManager.Entry> Entries { get; set; }

		[JsonSerializable("tcu", typeof(CloudUser))]
		public List<CloudUser> Users { get; set; }

		[JsonSerializable("e", null)]
		public int EndsInSeconds { get; set; }

		[JsonSerializable("c", null)]
		public int ClosesInSeconds { get; set; }

		[JsonSerializable("r", null)]
		public DateTime UtcReceived { get; set; }

		[JsonSerializable("pr", null)]
		public int PresentResult { get; set; }

		public bool IsFinalResult
		{
			get
			{
				return this.IsJoined && this.EndsInSeconds <= 0;
			}
		}

		public DateTime UtcEndTime
		{
			get
			{
				return this.UtcReceived + TimeSpan.FromSeconds((double)this.EndsInSeconds);
			}
		}

		public DateTime UtcCloseTime
		{
			get
			{
				return this.UtcReceived + TimeSpan.FromSeconds((double)this.ClosesInSeconds);
			}
		}

		public TournamentCloudManager.Type TournamentType
		{
			get
			{
				if (this.TournamentId >= 30000000)
				{
					return TournamentCloudManager.Type.Gold;
				}
				if (this.TournamentId >= 20000000)
				{
					return TournamentCloudManager.Type.Silver;
				}
				return TournamentCloudManager.Type.Bronze;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.PeriodId > 0;
			}
		}

		public bool IsJoined
		{
			get
			{
				return this.PeriodId > 0 && this.TournamentId > 0;
			}
		}

		public int GetScore(int leaderboard, string deviceId, string userId)
		{
			foreach (TournamentCloudManager.Entry entry in this.Entries)
			{
				if (entry.IsOwnedByDeviceOrUser(deviceId, userId))
				{
					string key = leaderboard.ToString();
					if (entry.Scores.ContainsKey(key))
					{
						return entry.Scores[key];
					}
				}
			}
			return 0;
		}

		public void SetScore(string deviceId, string userId, int leaderboard, int score)
		{
			foreach (TournamentCloudManager.Entry entry in this.Entries)
			{
				if (entry.IsOwnedByDeviceOrUser(deviceId, userId))
				{
					entry.Scores[leaderboard.ToString()] = score;
				}
			}
		}

		public CloudUser GetUser(string userId)
		{
			foreach (CloudUser cloudUser in this.Users)
			{
				if (userId == cloudUser.CloudId)
				{
					return cloudUser;
				}
			}
			return null;
		}
	}
}
