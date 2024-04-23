using System;
using System.Collections;
using System.Collections.Generic;
using Cloud;
using TactileModules.TactileCloud;

public class CloudInterface : CloudInterfaceBase, ICloudInterface
{
	public CloudInterface(string host, string packageName, int versionCode, string versionName, string sharedSecret, string deviceId, string apsEnvironment, IRequestMetaDataProviderRegistry requestMetaDataProviderRegistry) : base(host, packageName, versionCode, versionName, sharedSecret, deviceId, apsEnvironment, requestMetaDataProviderRegistry)
	{
	}

	public IEnumerator GetConfiguration(int version, string woogaId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"version",
				version
			}
		};
		if (!string.IsNullOrEmpty(woogaId))
		{
			hashtable["wid"] = woogaId;
		}
		return base.StartRequest("/game/v1/configuration/get", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator CreateUserSettings(string userId, Hashtable privateSettings, Hashtable publicSettings, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"userSettings",
				new Hashtable
				{
					{
						"privateSettings",
						privateSettings
					},
					{
						"publicSettings",
						publicSettings
					}
				}
			},
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/user-settings/create", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator UpdateUserSettings(string userId, Hashtable privateSettings, Hashtable publicSettings, int version, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"userSettings",
				new Hashtable
				{
					{
						"privateSettings",
						privateSettings
					},
					{
						"publicSettings",
						publicSettings
					},
					{
						"version",
						version
					}
				}
			},
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/user-settings/update", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator GetUserSettings(string userId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/user-settings/get", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator PatchUserSettings(string userId, Hashtable objPathsToSet, Hashtable objPathsToUnset, int version, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"version",
				version
			},
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		if (objPathsToSet.Count > 0)
		{
			hashtable["set"] = objPathsToSet;
		}
		if (objPathsToUnset.Count > 0)
		{
			hashtable["unset"] = objPathsToUnset;
		}
		return base.StartRequest("/game/v2/user-settings/patch", hashtable, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator GetOtherUserSettings(string otherUserId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"userId",
				otherUserId
			}
		};
		return base.StartRequest("/game/v1/user-settings/get-other", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator GetFriendsAndUserSettings(string userId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/user-settings/get-friends", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator LeaderboardsSubmitScore(string userId, int score, int leaderboard, int videoId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"score",
				new Hashtable
				{
					{
						"score",
						score
					},
					{
						"leaderboard",
						leaderboard
					},
					{
						"videoId",
						videoId
					}
				}
			},
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/leaderboards/submit", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator LeaderboardsGetScores(string userId, int leaderboard, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"leaderboard",
				leaderboard
			},
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						userId
					}
				}
			}
		};
		return base.StartRequest("/game/v1/leaderboards/get", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator TournamentStatus(string userId, Response result)
	{
		Hashtable hashtable = new Hashtable();
		if (!string.IsNullOrEmpty(userId))
		{
			hashtable["context"] = new Hashtable
			{
				{
					"userId",
					userId
				}
			};
		}
		return base.StartRequest("/game/v1/tournament/status", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator TournamentJoin(string userId, TournamentCloudManager.Type tournamentType, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"tournamentType",
				(int)tournamentType
			}
		};
		if (!string.IsNullOrEmpty(userId))
		{
			hashtable["context"] = new Hashtable
			{
				{
					"userId",
					userId
				}
			};
		}
		return base.StartRequest("/game/v1/tournament/join", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator TournamentSubmitScore(string userId, int periodId, int tournamentId, int leaderboard, int score, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"periodId",
				periodId
			},
			{
				"tournamentId",
				tournamentId
			},
			{
				"score",
				new Hashtable
				{
					{
						"score",
						score
					},
					{
						"leaderboard",
						leaderboard
					}
				}
			}
		};
		if (!string.IsNullOrEmpty(userId))
		{
			hashtable["context"] = new Hashtable
			{
				{
					"userId",
					userId
				}
			};
		}
		return base.StartRequest("/game/v1/tournament/submit", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator TournamentGetEntries(string userId, int periodId, int tournamentId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"periodId",
				periodId
			},
			{
				"tournamentId",
				tournamentId
			}
		};
		if (!string.IsNullOrEmpty(userId))
		{
			hashtable["context"] = new Hashtable
			{
				{
					"userId",
					userId
				}
			};
		}
		return base.StartRequest("/game/v1/tournament/get", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator TournamentPresent(string userId, int periodId, int tournamentId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"periodId",
				periodId
			},
			{
				"tournamentId",
				tournamentId
			}
		};
		if (!string.IsNullOrEmpty(userId))
		{
			hashtable["context"] = new Hashtable
			{
				{
					"userId",
					userId
				}
			};
		}
		return base.StartRequest("/game/v1/tournament/present", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public new IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"eventParams",
				eventParams
			}
		};
		if (base64EncodedTransactionReceipt != null)
		{
			hashtable["receiptData"] = base64EncodedTransactionReceipt;
		}
		return base.StartRequest("/purchase/v1/report", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public class UserSettingsResponse : Response
	{
		public CloudUserSettings UserSettings
		{
			get
			{
				Hashtable hashtable = base.data["userSettings"] as Hashtable;
				return (hashtable == null) ? null : JsonSerializer.HashtableToObject<CloudUserSettings>(hashtable);
			}
		}
	}

	public class FriendsAndUserSettingsResponse : Response
	{
		public IEnumerable<CloudUser> Friends
		{
			get
			{
				ArrayList array = (ArrayList)base.data["friends"];
				IEnumerator enumerator = array.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Hashtable ht = (Hashtable)obj;
						yield return JsonSerializer.HashtableToObject<CloudUser>(ht);
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
				yield break;
			}
		}

		public IEnumerable<CloudUserSettings> Settings
		{
			get
			{
				ArrayList array = (ArrayList)base.data["settings"];
				IEnumerator enumerator = array.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Hashtable ht = (Hashtable)obj;
						yield return JsonSerializer.HashtableToObject<CloudUserSettings>(ht);
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
				yield break;
			}
		}
	}

	public class LeaderboardsSubmitScoreResponse : Response
	{
		public CloudScore Score
		{
			get
			{
				return JsonSerializer.HashtableToObject<CloudScore>((Hashtable)base.data["score"]);
			}
		}
	}

	public class LeaderboardsGetScoresResponse : Response
	{
		public Dictionary<string, List<CloudScore>> Scores
		{
			get
			{
				Dictionary<string, List<CloudScore>> dictionary = new Dictionary<string, List<CloudScore>>();
				Hashtable hashtable = (Hashtable)base.data["scores"];
				IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
						string text = dictionaryEntry.Key as string;
						int leaderboard;
						if (int.TryParse(text, out leaderboard))
						{
							List<CloudScore> list = JsonSerializer.ArrayListToGenericList<CloudScore>(dictionaryEntry.Value as ArrayList);
							foreach (CloudScore cloudScore in list)
							{
								cloudScore.Leaderboard = leaderboard;
							}
							dictionary.Add(text, list);
						}
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
				return dictionary;
			}
		}
	}

	public class TournamentResponse : Response
	{
		public List<TournamentCloudManager.Entry> Entries
		{
			get
			{
				List<TournamentCloudManager.Entry> list = new List<TournamentCloudManager.Entry>();
				ArrayList arrayList = (ArrayList)base.data["entries"];
				IEnumerator enumerator = arrayList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Hashtable table = (Hashtable)obj;
						list.Add(JsonSerializer.HashtableToObject<TournamentCloudManager.Entry>(table));
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
				return list;
			}
		}

		public List<CloudUser> Users
		{
			get
			{
				List<CloudUser> list = new List<CloudUser>();
				ArrayList arrayList = (ArrayList)base.data["users"];
				IEnumerator enumerator = arrayList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Hashtable table = (Hashtable)obj;
						list.Add(JsonSerializer.HashtableToObject<CloudUser>(table));
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
				return list;
			}
		}

		public int PeriodId
		{
			get
			{
				return (int)((double)base.data["periodId"]);
			}
		}

		public int TournamentId
		{
			get
			{
				return (int)((double)base.data["tournamentId"]);
			}
		}

		public int TournamentEndsInSeconds
		{
			get
			{
				return (int)((double)base.data["ends"]);
			}
		}

		public int TournamentClosesInSeconds
		{
			get
			{
				return (int)((double)base.data["closes"]);
			}
		}
	}

	public class TournamentPresentResponse : CloudInterface.TournamentResponse
	{
		public TournamentCloudManager.PresentResult PresentResult
		{
			get
			{
				return (TournamentCloudManager.PresentResult)((double)base.data["presentResult"]);
			}
		}
	}

	public class PurchaseResponse : Response
	{
		public Hashtable Receipt
		{
			get
			{
				return (Hashtable)base.data["receipt"];
			}
		}

		public bool IsNew
		{
			get
			{
				return (bool)base.data["isNew"];
			}
		}
	}
}
