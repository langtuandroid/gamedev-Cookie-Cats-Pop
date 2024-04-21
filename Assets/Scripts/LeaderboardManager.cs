using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CollectionUtils;
using UnityEngine;

public class LeaderboardManager
{
	protected LeaderboardManager(CloudClient cloudClient)
	{
		this.cloudClient = cloudClient;
		this.localScores = this.PersistedLocalScores;
		this.cachedCloudScores = this.PersistedCachedCloudScores;
		cloudClient.UserChanged += this.UserChangedHandler;
	}

	public static LeaderboardManager Instance { get; private set; }

	public static LeaderboardManager CreateInstance(CloudClient cloudClient)
	{
		LeaderboardManager.Instance = new LeaderboardManager(cloudClient);
		return LeaderboardManager.Instance;
	}

	~LeaderboardManager()
	{
		this.cloudClient.UserChanged -= this.UserChangedHandler;
	}

	private bool IsError(object error)
	{
		return error != null;
	}

	private void UserChangedHandler(CloudUser user)
	{
		if (this.cloudClient.HasValidUser)
		{
			FiberCtrl.Pool.Run(this.SubmitLocalScoresCr(), false);
		}
		else
		{
			this.PersistedCachedCloudScores = null;
			this.cachedCloudScores = this.PersistedCachedCloudScores;
		}
	}

	public void SubmitVideoIdForScore(int score, int leaderboard, int videoId)
	{
		if (!this.localScores.ContainsKey(leaderboard))
		{
			return;
		}
		LocalScore localScore = this.localScores[leaderboard];
		CloudScore cloudScore = (!this.cloudClient.HasValidUser) ? null : this.GetCachedCloudScore(this.cloudClient.CachedMe.CloudId, leaderboard);
		if (localScore.Score == score && localScore.Leaderboard == leaderboard && (cloudScore == null || score >= cloudScore.Score))
		{
			localScore.VideoId = videoId;
			localScore.SubmittedToCloud = false;
			this.PersistedLocalScores = this.localScores;
		}
		if (this.cloudClient.HasValidUser)
		{
			FiberCtrl.Pool.Run(this.SubmitLocalScoresCr(), false);
		}
	}

	public void SubmitScore(int score, int leaderboard)
	{
		if (!this.localScores.ContainsKey(leaderboard))
		{
			LocalScore localScore = new LocalScore();
			localScore.Leaderboard = leaderboard;
			this.localScores.Add(leaderboard, localScore);
		}
		LocalScore localScore2 = this.localScores[leaderboard];
		CloudScore cloudScore = (!this.cloudClient.HasValidUser) ? null : this.GetCachedCloudScore(this.cloudClient.CachedMe.CloudId, leaderboard);
		if (score > localScore2.Score && (cloudScore == null || score > cloudScore.Score))
		{
			if (cloudScore != null)
			{
				cloudScore.Score = score;
				cloudScore.VideoId = 0;
				this.cachedCloudScores[leaderboard].Sort((CloudScore x, CloudScore y) => y.Score.CompareTo(x.Score));
				this.PersistedCachedCloudScores = this.cachedCloudScores;
			}
			else if (this.cloudClient.HasValidUser)
			{
				cloudScore = new CloudScore();
				cloudScore.Score = score;
				cloudScore.Leaderboard = leaderboard;
				cloudScore.UserId = this.cloudClient.CachedMe.CloudId;
				if (!this.cachedCloudScores.ContainsKey(leaderboard))
				{
					this.cachedCloudScores[leaderboard] = new List<CloudScore>();
				}
				this.cachedCloudScores[leaderboard].Add(cloudScore);
				this.cachedCloudScores[leaderboard].Sort((CloudScore x, CloudScore y) => y.Score.CompareTo(x.Score));
				this.PersistedCachedCloudScores = this.cachedCloudScores;
			}
			localScore2.Score = score;
			localScore2.SubmittedToCloud = false;
			this.PersistedLocalScores = this.localScores;
			if (this.cloudClient.HasValidUser)
			{
				FiberCtrl.Pool.Run(this.SubmitLocalScoresCr(), false);
			}
		}
	}

	private LocalScore GetUnsubmittedLocalScore()
	{
		foreach (KeyValuePair<int, LocalScore> keyValuePair in this.localScores)
		{
			if (!keyValuePair.Value.SubmittedToCloud)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public IEnumerator SubmitLocalScoresCr()
	{
		if (this.submitLocalScoresInProgress)
		{
			yield break;
		}
		this.submitLocalScoresInProgress = true;
		yield return this.SubmitLocalScoresInternal();
		this.submitLocalScoresInProgress = false;
		yield break;
	}

	private IEnumerator SubmitLocalScoresInternal()
	{
		for (LocalScore localScore = this.GetUnsubmittedLocalScore(); localScore != null; localScore = this.GetUnsubmittedLocalScore())
		{
			int score = localScore.Score;
			int videoId = localScore.VideoId;
			object error = null;
			IEnumerator e = this.cloudClient.SubmitScore(score, localScore.Leaderboard, videoId, delegate(object err, CloudScore updatedCloudScore)
			{
				error = err;
				if (err == null)
				{
					CloudScore cachedCloudScore = this.GetCachedCloudScore(updatedCloudScore.UserId, updatedCloudScore.Leaderboard);
					if (cachedCloudScore != null && updatedCloudScore.Score > cachedCloudScore.Score)
					{
						cachedCloudScore.Score = updatedCloudScore.Score;
						this.PersistedCachedCloudScores = this.cachedCloudScores;
					}
				}
			});
			while (e.MoveNext())
			{
				object obj = e.Current;
				yield return obj;
			}
			if (this.IsError(error))
			{
				yield break;
			}
			if (score == localScore.Score)
			{
				localScore.SubmittedToCloud = true;
				this.PersistedLocalScores = this.localScores;
			}
		}
		yield break;
	}

	public List<CloudScore> GetCachedCloudScores(int leaderboard)
	{
		if (this.cachedCloudScores.ContainsKey(leaderboard))
		{
			return this.cachedCloudScores[leaderboard];
		}
		return null;
	}

	private CloudScore GetCachedCloudScore(string userId, int leaderboard)
	{
		if (this.cachedCloudScores.ContainsKey(leaderboard))
		{
			foreach (CloudScore cloudScore in this.cachedCloudScores[leaderboard])
			{
				if (cloudScore.UserId == userId)
				{
					return cloudScore;
				}
			}
		}
		return null;
	}

	private LocalScore GetLocalScore(int leaderboard)
	{
		if (this.localScores.ContainsKey(leaderboard))
		{
			return this.localScores[leaderboard];
		}
		return null;
	}

	public int GetNumberOfScores(int leaderboard)
	{
		int num = 0;
		if (this.localScores != null)
		{
			num = Mathf.Max(num, this.localScores.Count);
		}
		if (this.cloudClient.HasValidUser && this.cachedCloudScores != null)
		{
			num = Mathf.Max(num, this.cachedCloudScores.Count);
		}
		return num;
	}

	public int GetBestScore(int leaderboard)
	{
		int num = 0;
		LocalScore localScore = this.GetLocalScore(leaderboard);
		if (localScore != null)
		{
			num = localScore.Score;
		}
		CloudScore cloudScore = (!this.cloudClient.HasValidUser) ? null : this.GetCachedCloudScore(this.cloudClient.CachedMe.CloudId, leaderboard);
		if (cloudScore != null && cloudScore.Score > num)
		{
			num = cloudScore.Score;
		}
		return num;
	}

	public IEnumerator UpdateCachedCloudScoresCr(int leaderboard)
	{
		IEnumerator e = this.cloudClient.GetScores(leaderboard, delegate(object err, Dictionary<string, List<CloudScore>> scores)
		{
			if (err == null)
			{
				foreach (KeyValuePair<string, List<CloudScore>> keyValuePair in scores)
				{
					this.UpdateCachedScoresForLeaderboard(int.Parse(keyValuePair.Key), keyValuePair.Value);
				}
				this.PersistedCachedCloudScores = this.cachedCloudScores;
			}
		});
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		yield break;
	}

	private void UpdateCachedScoresForLeaderboard(int leaderboard, List<CloudScore> scores)
	{
		if (!this.cachedCloudScores.ContainsKey(leaderboard))
		{
			this.cachedCloudScores.Add(leaderboard, scores);
		}
		else
		{
			this.cachedCloudScores[leaderboard] = scores;
		}
		LocalScore localScore = this.GetLocalScore(leaderboard);
		if (localScore != null && !localScore.SubmittedToCloud && this.cloudClient.HasValidUser)
		{
			CloudScore cloudScore = this.GetCachedCloudScore(this.cloudClient.CachedMe.CloudId, leaderboard);
			if (cloudScore != null)
			{
				if (localScore.Score > cloudScore.Score)
				{
					cloudScore.Score = localScore.Score;
					this.cachedCloudScores[leaderboard].Sort((CloudScore x, CloudScore y) => y.Score.CompareTo(x.Score));
				}
			}
			else
			{
				cloudScore = new CloudScore();
				cloudScore.Score = localScore.Score;
				cloudScore.Leaderboard = leaderboard;
				cloudScore.UserId = this.cloudClient.CachedMe.CloudId;
				this.cachedCloudScores[leaderboard].Add(cloudScore);
				this.cachedCloudScores[leaderboard].Sort((CloudScore x, CloudScore y) => y.Score.CompareTo(x.Score));
			}
		}
	}

	private Dictionary<int, LocalScore> PersistedLocalScores
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("LeaderboardManagerLocalScores", string.Empty);
			Dictionary<int, LocalScore> dictionary = new Dictionary<int, LocalScore>(IntComparer.Instance);
			if (securedString.Length > 0)
			{
				Hashtable hashtable = securedString.hashtableFromJson();
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						dictionary.Add(int.Parse((string)dictionaryEntry.Key), JsonSerializer.HashtableToObject<LocalScore>((Hashtable)dictionaryEntry.Value));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			return dictionary;
		}
		set
		{
			if (value != null)
			{
				Hashtable hashtable = new Hashtable();
				foreach (KeyValuePair<int, LocalScore> keyValuePair in value)
				{
					hashtable.Add(keyValuePair.Key, JsonSerializer.ObjectToHashtable(keyValuePair.Value));
				}
				TactilePlayerPrefs.SetSecuredString("LeaderboardManagerLocalScores", hashtable.toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("LeaderboardManagerLocalScores", string.Empty);
			}
		}
	}

	private Dictionary<int, List<CloudScore>> PersistedCachedCloudScores
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("LeaderboardManagerCachedCloudScores", string.Empty);
			Dictionary<int, List<CloudScore>> dictionary = new Dictionary<int, List<CloudScore>>(IntComparer.Instance);
			if (securedString.Length > 0)
			{
				Hashtable hashtable = securedString.hashtableFromJson();
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						dictionary.Add(int.Parse((string)dictionaryEntry.Key), JsonSerializer.ArrayListToGenericList<CloudScore>((ArrayList)dictionaryEntry.Value));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			return dictionary;
		}
		set
		{
			if (value != null)
			{
				Hashtable hashtable = new Hashtable();
				foreach (KeyValuePair<int, List<CloudScore>> keyValuePair in value)
				{
					hashtable.Add(keyValuePair.Key, JsonSerializer.GenericListToArrayList<CloudScore>(keyValuePair.Value));
				}
				TactilePlayerPrefs.SetSecuredString("LeaderboardManagerCachedCloudScores", hashtable.toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("LeaderboardManagerCachedCloudScores", string.Empty);
			}
			this.OnCachedCloudScoresChanged();
		}
	}

	private bool IsOwnScore(CloudScore score)
	{
		return this.cloudClient.CachedMe != null && score.UserId == this.cloudClient.CachedMe.CloudId;
	}

	public CloudScore GetFirstFriendScoreBetterThan(int score, int leaderboard)
	{
		if (this.cachedCloudScores.ContainsKey(leaderboard))
		{
			foreach (CloudScore cloudScore in this.cachedCloudScores[leaderboard])
			{
				if (cloudScore.Score > score && !this.IsOwnScore(cloudScore))
				{
					return cloudScore;
				}
			}
		}
		return null;
	}

	public int GetPlacement(int leaderboard)
	{
		if (!this.cachedCloudScores.ContainsKey(leaderboard))
		{
			return -1;
		}
		List<CloudScore> list = this.cachedCloudScores[leaderboard];
		for (int i = 0; i < list.Count; i++)
		{
			if (this.IsOwnScore(list[i]))
			{
				return i + 1;
			}
		}
		return -1;
	}

	private void OnCachedCloudScoresChanged()
	{
		if (this.CachedCloudScoresChanged != null)
		{
			this.CachedCloudScoresChanged();
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action CachedCloudScoresChanged;

	private bool submitLocalScoresInProgress;

	private const string PREFS_LOCAL_SCORES = "LeaderboardManagerLocalScores";

	private const string PREFS_CACHED_CLOUD_SCORES = "LeaderboardManagerCachedCloudScores";

	private CloudClient cloudClient;

	private Dictionary<int, LocalScore> localScores;

	private Dictionary<int, List<CloudScore>> cachedCloudScores;
}
