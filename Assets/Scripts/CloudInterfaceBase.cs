using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Cloud;
using Fibers;
using TactileModules.TactileCloud;
using UnityEngine;

public class CloudInterfaceBase : ICloudInterfaceBase, ICloudResponseEvents
{
	public CloudInterfaceBase(string host, string packageName, int versionCode, string versionName, string sharedGameSecret, string deviceUid, string apsEnvironment, IRequestMetaDataProviderRegistry requestMetaDataProviderRegistry)
	{
		this.host = host;
		this.packageName = packageName;
		this.versionCode = versionCode;
		this.versionName = versionName;
		this.sharedGameSecret = sharedGameSecret;
		this.deviceUid = deviceUid;
		this.requestRunning = false;
		this.userAuthSecret = this.PersistedUserAuthSecret;
		this.nonceSession = CloudInterfaceBase.PersistedNonceSession;
		this.nonceValue = CloudInterfaceBase.PersistedNonceValue;
		this.serverTime = CloudInterfaceBase.PersistedServerTime;
		this.serverTimeReceived = CloudInterfaceBase.PersistedServerTimeReceived;
		this.sharedSecretPart1Device = Encoding.UTF8.GetBytes(deviceUid.Substring(0, deviceUid.Length / 2) + sharedGameSecret.Substring(0, sharedGameSecret.Length / 2));
		this.sharedSecretPart1User = Encoding.UTF8.GetBytes(this.userAuthSecret.Substring(0, this.userAuthSecret.Length / 2) + sharedGameSecret.Substring(0, sharedGameSecret.Length / 2));
		this.sharedSecretPart3Device = Encoding.UTF8.GetBytes(deviceUid.Substring(deviceUid.Length / 2) + sharedGameSecret.Substring(sharedGameSecret.Length / 2));
		this.sharedSecretPart3User = Encoding.UTF8.GetBytes(this.userAuthSecret.Substring(this.userAuthSecret.Length / 2) + sharedGameSecret.Substring(sharedGameSecret.Length / 2));
		this.requestPriorityQueue = new RequestPriorityQueue();
		this.requestMetaDataProviderRegistry = requestMetaDataProviderRegistry;
	}

	////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Hashtable, string> ResponseMetaDataReceived = delegate (Hashtable A_0, string A_1)
    {
    };



    public string Host
	{
		get
		{
			return this.host;
		}
		set
		{
			this.host = value;
		}
	}

	public string UserAuthSecret
	{
		set
		{
			if (value == null)
			{
			}
			this.userAuthSecret = value;
			this.PersistedUserAuthSecret = value;
			this.sharedSecretPart1User = Encoding.UTF8.GetBytes(this.userAuthSecret.Substring(0, this.userAuthSecret.Length / 2) + this.sharedGameSecret.Substring(0, this.sharedGameSecret.Length / 2));
			this.sharedSecretPart3User = Encoding.UTF8.GetBytes(this.userAuthSecret.Substring(this.userAuthSecret.Length / 2) + this.sharedGameSecret.Substring(this.sharedGameSecret.Length / 2));
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnServerTimeUpdated;

	public int LastReceivedServerTimeUnixEpocUTC
	{
		get
		{
			return this.serverTime;
		}
	}

	public int ClientAdjustedServerTimeUnixEpocUTC
	{
		get
		{
			if (this.serverTime == 0 || this.serverTimeReceived == 0)
			{
				return 0;
			}
			int num = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			if (num < this.serverTimeReceived)
			{
				this.serverTime = 0;
				this.serverTimeReceived = 0;
				CloudInterfaceBase.PersistedServerTime = 0;
				CloudInterfaceBase.PersistedServerTimeReceived = 0;
				return 0;
			}
			return this.serverTime + (num - this.serverTimeReceived);
		}
	}

	private Hashtable GetDeviceHashtable(CloudLocalDevice device)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"uid",
				device.UID
			},
			{
				"type",
				device.type
			},
			{
				"systemLanguage",
				device.language
			},
			{
				"pushEnabled",
				device.pushEnabled
			}
		};
		if (!string.IsNullOrEmpty(device.IFA))
		{
			hashtable["ifa"] = device.IFA;
		}
		if (!string.IsNullOrEmpty(device.AID))
		{
			hashtable["aid"] = device.AID;
		}
		if (!string.IsNullOrEmpty(device.gameThriveId))
		{
			hashtable["gameThriveId"] = device.gameThriveId;
		}
		return hashtable;
	}

	public IEnumerator CreateOrUpdateDevice(CloudLocalDevice device, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"device",
				this.GetDeviceHashtable(device)
			}
		};
		return this.StartRequest("/devices/v1/create-or-update", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator CreateOrUpdateUser(Hashtable userData, Response result)
	{
		return this.StartRequest("/users/v3/create-or-update", userData, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator LogoutUserFromDevice(string userId, Response result)
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
		return this.StartRequest("/users/v1/logout-from-device", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator DeleteCloudUser(string cloudId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"context",
				new Hashtable
				{
					{
						"userId",
						cloudId
					}
				}
			}
		};
		return this.StartRequest("/users/v1/delete", json, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator SendPush(string userId, string receiverId, string message, Dictionary<string, string> payload, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"receiverId",
				receiverId
			},
			{
				"message",
				message
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
		if (payload != null)
		{
			hashtable["payload"] = payload;
		}
		return this.StartRequest("/users/v1/send-push", hashtable, CloudInterfaceBase.AuthMode.User, result, RequestPriority.Default);
	}

	public IEnumerator GetConfiguration(int version, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"version",
				version
			}
		};
		return this.StartRequest("/game/v2/configuration/get", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator GetAssetBundles(int version, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"version",
				version
			}
		};
		return this.StartRequest("/game/v1/asset-bundles/get", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Response result)
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
		return this.StartRequest("/purchase/v1/report", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator AdsGetEnabled(Response result)
	{
		return this.StartRequest("/ads/v1/get-enabled", new Hashtable(), CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator AdsGetPriority(Response result)
	{
		return this.StartRequest("/ads/v1/get-priority", new Hashtable(), CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator AdsReportImpression(string adType, string adLocation, string provider, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"type",
				adType
			},
			{
				"location",
				adLocation
			},
			{
				"provider",
				provider
			}
		};
		return this.StartRequest("/ads/v1/report-impression", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator AdsReportRequest(string adType, string adLocation, string provider, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"type",
				adType
			},
			{
				"location",
				adLocation
			},
			{
				"provider",
				provider
			}
		};
		return this.StartRequest("/ads/v1/report-request", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator ReportAdjustIOAttribution(Hashtable data, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"data",
				data
			}
		};
		return this.StartRequest("/adjust/report-attribution", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator CheckFacebookPayment(long paymentId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"paymentId",
				paymentId
			}
		};
		return this.StartRequest("/facebook/check-payment", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UploadBackupUserSettings(string userSettingsJson, string deviceId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"userSettings",
				userSettingsJson
			},
			{
				"deviceId",
				deviceId
			}
		};
		return this.StartRequest("/devices/v1/backup/save", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator DownloadBackupUserSettings(string deviceId, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"deviceId",
				deviceId
			}
		};
		return this.StartRequest("/devices/v1/backup/restore", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator GetFeatures(string userId, Hashtable targetingParams, Dictionary<string, int> metaDataVersions, List<string> activeFeatureIds, Response result)
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
		Hashtable hashtable2 = new Hashtable();
		foreach (KeyValuePair<string, int> keyValuePair in metaDataVersions)
		{
			hashtable2.Add(keyValuePair.Key, keyValuePair.Value);
		}
		hashtable["acceptedVersions"] = hashtable2;
		hashtable["targetingParameters"] = targetingParams;
		hashtable["running"] = JsonSerializer.GenericListToArrayList<string>(activeFeatureIds);
		return this.StartRequest("/game/v2/scheduled-features/get", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator StarTournamentJoin(string userId, string featureId, int farthestLevelIndex, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"level",
				farthestLevelIndex
			},
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v2/star-tournament/join", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator StarTournamentStatus(string userId, string featureId, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/star-tournament/status", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator StarTournamentSubmitScore(string userId, string featureId, int farthestUnlockLevel, int stars, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
			},
			{
				"levelId",
				farthestUnlockLevel
			},
			{
				"stars",
				stars
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
		return this.StartRequest("/game/v2/star-tournament/submit-score", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator StarTournamentPresent(string userId, string featureId, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/star-tournament/present", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator SocialChallengeStatus(string userId, bool isPlaying, int challengeVersion, DateTime expirationDate, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"isPlaying",
				isPlaying
			},
			{
				"challengeVersion",
				challengeVersion
			},
			{
				"expirationDate",
				expirationDate.ToString()
			}
		};
		hashtable["context"] = new Hashtable
		{
			{
				"userId",
				userId
			}
		};
		return this.StartRequest("/game/v1/social-challenge/status", hashtable, CloudInterfaceBase.AuthMode.User, result, requestPriority);
	}

	public IEnumerator SocialChallengeReseat(string userId, int challengeVersion, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"challengeVersion",
				challengeVersion
			}
		};
		hashtable["context"] = new Hashtable
		{
			{
				"userId",
				userId
			}
		};
		return this.StartRequest("/game/v1/social-challenge/reseat", hashtable, CloudInterfaceBase.AuthMode.User, result, requestPriority);
	}

	public IEnumerator SocialChallengeSubmitLevelCompleted(string userId, int challengeVersion, int levelId, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"challengeVersion",
				challengeVersion
			},
			{
				"levelId",
				levelId
			}
		};
		hashtable["context"] = new Hashtable
		{
			{
				"userId",
				userId
			}
		};
		return this.StartRequest("/game/v1/social-challenge/submitLevel", hashtable, CloudInterfaceBase.AuthMode.User, result, requestPriority);
	}

	public IEnumerator SocialChallengeSubmitChestOpened(string userId, int challengeVersion, int chestRank, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"challengeVersion",
				challengeVersion
			},
			{
				"chestRank",
				chestRank
			}
		};
		hashtable["context"] = new Hashtable
		{
			{
				"userId",
				userId
			}
		};
		return this.StartRequest("/game/v1/social-challenge/submitChest", hashtable, CloudInterfaceBase.AuthMode.User, result, requestPriority);
	}

	public IEnumerator SocialChallengeGetOtherPlayers(string userId, int challengeVersion, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"challengeVersion",
				challengeVersion
			}
		};
		hashtable["context"] = new Hashtable
		{
			{
				"userId",
				userId
			}
		};
		return this.StartRequest("/game/v1/social-challenge/get", hashtable, CloudInterfaceBase.AuthMode.User, result, requestPriority);
	}

	public IEnumerator LevelDashJoinRequest(string userId, string featureId, int completedLevelHumanNumber, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"level",
				completedLevelHumanNumber
			},
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/level-dash/join", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator LevelDashStatusRequest(string userId, string featureId, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/level-dash/status", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator LevelDashSubmitScoreRequest(string userId, string featureId, int completedLevelHumanNumber, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
			},
			{
				"levelId",
				completedLevelHumanNumber
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
		return this.StartRequest("/game/v1/level-dash/submit-score", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator LevelDashGetRewardRequest(string userId, string featureId, RequestPriority requestPriority, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/level-dash/reward-status", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator UserSupportSubmitMessage(string userId, string message, string email, string name, Hashtable metaData, Response result, string messageContext = "", ArrayList files = null)
	{
		ArrayList value = files ?? new ArrayList();
		Hashtable hashtable = new Hashtable
		{
			{
				"message",
				new Hashtable
				{
					{
						"body",
						message
					},
					{
						"senderEmail",
						email
					},
					{
						"senderName",
						name
					},
					{
						"meta",
						metaData
					},
					{
						"context",
						messageContext
					},
					{
						"fileAttachments",
						value
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
		return this.StartRequest("/game/v1/user-support-conversations/submit", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportGetMessages(string userId, Response result)
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
		return this.StartRequest("/game/v1/user-support-conversations/get-all", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportGetArticles(string userId, Response result)
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
		return this.StartRequest("/game/v1/user-support-articles/list", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportSetRead(string userId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"read",
				true
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
		return this.StartRequest("/game/v1/user-support-conversations/set-conversation-read", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportClaimAttachments(string userId, Response result)
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
		return this.StartRequest("/game/v1/user-support-conversations/claim-all-attachments", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportClaimMessageAttachments(string userId, string messageId, string[] attachmentNames, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"messageId",
				messageId
			},
			{
				"attachmentNames",
				attachmentNames
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
		return this.StartRequest("/game/v1/user-support-conversations/claim-attachments", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportCheckUnread(string userId, Response result)
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
		return this.StartRequest("/game/v1/user-support-conversations/check-unread", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportDismissedBackup(string messageId, string userId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"messageId",
				messageId
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
		return this.StartRequest("/game/v1/user-support-conversations/dismiss-backup", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportAppliedBackup(string messageId, string userId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"messageId",
				messageId
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
		return this.StartRequest("/game/v1/user-support-conversations/apply-backup", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator UserSupportGetPartialImageUploadRequestParameters(string userId, Response result)
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
		return this.StartRequest("/game/v1/user-support-conversations/generate-partial-upload-request", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator EndlessChallengeJoin(string userId, string featureId, int farthestLevelIndex, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
			}
		};
		hashtable["joinparams"] = new Hashtable
		{
			{
				"level",
				farthestLevelIndex
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
		return this.StartRequest("/game/v1/generic-tournament/join", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Interactive);
	}

	public IEnumerator EndlessChallengeStatus(string userId, string featureId, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return this.StartRequest("/game/v1/generic-tournament/status", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator EndlessChallengeSubmitScore(string userId, string featureId, int maxRows, Response result)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
			}
		};
		hashtable["score"] = new Hashtable
		{
			{
				"maxRows",
				maxRows
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
		return this.StartRequest("/game/v1/generic-tournament/submit", hashtable, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator GenericTournamentJoin(string userId, string featureId, Hashtable joinParams, Hashtable score, Response result, RequestPriority requestPriority)
	{
		Hashtable hashtable = this.MakeCommonGenericTournamentArgs(featureId, userId);
		hashtable["joinparams"] = joinParams;
		hashtable["score"] = score;
		return this.StartRequest("/game/v1/generic-tournament/join", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator GenericTournamentStatus(string userId, string featureId, Response result, RequestPriority requestPriority)
	{
		Hashtable json = this.MakeCommonGenericTournamentArgs(featureId, userId);
		return this.StartRequest("/game/v1/generic-tournament/status", json, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator GenericTournamentSubmitScore(string userId, string featureId, Hashtable score, Response result, RequestPriority requestPriority)
	{
		Hashtable hashtable = this.MakeCommonGenericTournamentArgs(featureId, userId);
		hashtable["score"] = score;
		return this.StartRequest("/game/v1/generic-tournament/submit", hashtable, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator GenericTournamentPresent(string userId, string featureId, Response result, RequestPriority requestPriority)
	{
		Hashtable json = this.MakeCommonGenericTournamentArgs(featureId, userId);
		return this.StartRequest("/game/v1/generic-tournament/present", json, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	private Hashtable MakeCommonGenericTournamentArgs(string featureId, string userId)
	{
		Hashtable hashtable = new Hashtable
		{
			{
				"featureId",
				featureId
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
		return hashtable;
	}

	public IEnumerator GenericTournamentGetPastUnclaimed(string featureType, Response result, RequestPriority requestPriority)
	{
		Hashtable json = new Hashtable
		{
			{
				"kind",
				featureType
			}
		};
		return this.StartRequest("/game/v1/generic-tournament/get-past-unclaimed", json, CloudInterfaceBase.AuthMode.Device, result, requestPriority);
	}

	public IEnumerator LevelRatingGetRatings(string[] levelContentHashes, Response result)
	{
		Hashtable json = new Hashtable
		{
			{
				"levels",
				levelContentHashes
			}
		};
		return this.StartRequest("/game/v1/level-rating/get", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	public IEnumerator GetAbTests(List<string> activeTestIds, List<string> supportedTestTypeIds, Hashtable targetingParams, ICloudResponse result)
	{
		Hashtable json = new Hashtable
		{
			{
				"running",
				JsonSerializer.GenericListToArrayList<string>(activeTestIds)
			},
			{
				"supportedHashes",
				JsonSerializer.GenericListToArrayList<string>(supportedTestTypeIds)
			},
			{
				"targetingParameters",
				targetingParams
			}
		};
		return this.StartRequest("/game/v1/ab-tests/get", json, CloudInterfaceBase.AuthMode.Device, result, RequestPriority.Default);
	}

	private void LogError(ReturnCode returnCode, string resultText)
	{
		switch (returnCode + 15)
		{
		case ReturnCode.NoError:
			break;
		case ReturnCode.NoErrorAlreadyLatestVersion:
			break;
		case (ReturnCode)2:
			break;
		case (ReturnCode)3:
			break;
		default:
			if (returnCode != ReturnCode.ClientConnectionError)
			{
				if (returnCode != ReturnCode.DevelopmentNotImplemented)
				{
				}
			}
			break;
		case (ReturnCode)6:
			break;
		case (ReturnCode)7:
			break;
		case (ReturnCode)8:
			break;
		case (ReturnCode)9:
			break;
		case (ReturnCode)10:
			break;
		case (ReturnCode)11:
			break;
		case (ReturnCode)12:
			break;
		case (ReturnCode)13:
			break;
		case (ReturnCode)14:
			break;
		}
	}

	private byte[] GetSharedSecretPart1ForMode(CloudInterfaceBase.AuthMode mode)
	{
		if (mode == CloudInterfaceBase.AuthMode.Device)
		{
			return this.sharedSecretPart1Device;
		}
		if (mode == CloudInterfaceBase.AuthMode.User)
		{
			return this.sharedSecretPart1User;
		}
		return new byte[0];
	}

	private byte[] GetSharedSecretPart3ForMode(CloudInterfaceBase.AuthMode mode)
	{
		if (mode == CloudInterfaceBase.AuthMode.Device)
		{
			return this.sharedSecretPart3Device;
		}
		if (mode == CloudInterfaceBase.AuthMode.User)
		{
			return this.sharedSecretPart3User;
		}
		return new byte[0];
	}

	private void UpdateSharedSecret(CloudInterfaceBase.AuthMode mode)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(this.nonceValue);
		byte[] sharedSecretPart1ForMode = this.GetSharedSecretPart1ForMode(mode);
		byte[] sharedSecretPart3ForMode = this.GetSharedSecretPart3ForMode(mode);
		int num = sharedSecretPart1ForMode.Length + bytes.Length + sharedSecretPart3ForMode.Length;
		if (this.sharedSecret == null || this.sharedSecret.Length != num)
		{
			this.sharedSecret = new byte[num];
		}
		Buffer.BlockCopy(sharedSecretPart1ForMode, 0, this.sharedSecret, 0, sharedSecretPart1ForMode.Length);
		Buffer.BlockCopy(bytes, 0, this.sharedSecret, sharedSecretPart1ForMode.Length, bytes.Length);
		Buffer.BlockCopy(sharedSecretPart3ForMode, 0, this.sharedSecret, sharedSecretPart1ForMode.Length + bytes.Length, sharedSecretPart3ForMode.Length);
	}

	private string CalculateSignature(string url, string jsonText, CloudInterfaceBase.AuthMode mode)
	{
		this.UpdateSharedSecret(mode);
		HMACSHA256 hmacsha = new HMACSHA256(this.sharedSecret);
		string s = url + jsonText;
		byte[] array = hmacsha.ComputeHash(Encoding.UTF8.GetBytes(s));
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	private IEnumerator RunRequest(string url, Hashtable json, CloudInterfaceBase.AuthMode mode, ICloudResponse response, RequestPriority requestPriority)
	{
		json["nonceSession"] = this.nonceSession;
		json["nonceValue"] = this.nonceValue;
		json["cnonce"] = UnityEngine.Random.Range(0, int.MaxValue);
		string jsonText = MiniJSON.jsonEncode(json, false, 0);
		Dictionary<string, string> httpHeaders = new Dictionary<string, string>();
		httpHeaders.Add("Content-Type", "application/json");
		if (mode != CloudInterfaceBase.AuthMode.None)
		{
			httpHeaders.Add("x-cloud-signature", this.CalculateSignature(url, jsonText, mode));
		}
		yield return TactileRequest.Run(this.host + url, Encoding.UTF8.GetBytes(jsonText), httpHeaders, (TactileRequest.RequestPriority)requestPriority, delegate(WWW request)
		{
			response.FillResponse(request);
		});
		yield break;
	}

	protected IEnumerator StartRequest(string url, Hashtable json, CloudInterfaceBase.AuthMode mode, ICloudResponse result, RequestPriority requestPriority = RequestPriority.Default)
	{
		if (!json.ContainsKey("context"))
		{
			json["context"] = new Hashtable();
		}
		this.AttachMetaDataToRequest(json, url);
		Hashtable context = json["context"] as Hashtable;
		context["packageName"] = this.packageName;
		context["versionCode"] = this.versionCode;
		context["versionName"] = this.versionName;
		if (url != "/devices/v1/create-or-update")
		{
			context["deviceUid"] = this.deviceUid;
		}
		string requestId = Guid.NewGuid().ToString();
		this.requestPriorityQueue.Enqueue(requestPriority, requestId);
		yield return new Fiber.OnTerminate(delegate()
		{
			this.requestPriorityQueue.Remove(requestPriority, requestId);
		});
		while (this.requestRunning || !this.requestPriorityQueue.IsNextRequest(requestId))
		{
			yield return null;
		}
		this.requestRunning = true;
		yield return new Fiber.OnTerminate(delegate()
		{
			this.requestPriorityQueue.Remove(requestPriority, requestId);
			this.requestRunning = false;
		});
		int retryCount = 2;
		bool retryAllowed = true;
		while (retryCount > 0 && retryAllowed)
		{
			retryCount--;
			retryAllowed = false;
			result.data = null;
			yield return this.RunRequest(url, json, mode, result, requestPriority);
			if (result.data.Contains("nonce"))
			{
				Hashtable hashtable = (Hashtable)result.data["nonce"];
				this.nonceSession = (string)hashtable["session"];
				this.nonceValue = (string)hashtable["value"];
				CloudInterfaceBase.PersistedNonceSession = this.nonceSession;
				CloudInterfaceBase.PersistedNonceValue = this.nonceValue;
			}
			if (result.data.Contains("time"))
			{
				this.serverTime = (int)((double)result.data["time"]);
				this.serverTimeReceived = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
				CloudInterfaceBase.PersistedServerTime = this.serverTime;
				if (this.OnServerTimeUpdated != null)
				{
					this.OnServerTimeUpdated(this.serverTime);
				}
				CloudInterfaceBase.PersistedServerTimeReceived = this.serverTimeReceived;
			}
			ReturnCode returnCode = result.ReturnCode;
			if (result.ReturnCode == ReturnCode.NoError && result.data.ContainsKey("metaData"))
			{
				Hashtable hashtable2 = result.data["metaData"] as Hashtable;
				if (hashtable2 != null && hashtable2.Count > 0)
				{
					this.ResponseMetaDataReceived(hashtable2, url);
				}
			}
			if (returnCode != ReturnCode.AuthenticationFailedInvalidNonce)
			{
				if (returnCode < ReturnCode.NoError)
				{
					this.LogError(returnCode, result.Body);
				}
			}
			else if (retryCount > 0)
			{
				retryAllowed = true;
			}
		}
		this.requestPriorityQueue.Remove(requestPriority, requestId);
		this.requestRunning = false;
		yield break;
	}

	private void AttachMetaDataToRequest(Hashtable json, string endPoint)
	{
		IEnumerable<IRequestMetaDataProvider> providers = this.requestMetaDataProviderRegistry.GetProviders(endPoint);
		foreach (IRequestMetaDataProvider requestMetaDataProvider in providers)
		{
			string metaDataKey = requestMetaDataProvider.GetMetaDataKey();
			object metaDataValue = requestMetaDataProvider.GetMetaDataValue(endPoint);
			json.Add(metaDataKey, metaDataValue);
		}
	}

	private string PersistedUserAuthSecret
	{
		get
		{
			return TactilePlayerPrefs.GetString("userAuthSecret", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("userAuthSecret", value);
		}
	}

	private static string PersistedNonceSession
	{
		get
		{
			return TactilePlayerPrefs.GetString("nonceSession", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("nonceSession", value);
		}
	}

	private static string PersistedNonceValue
	{
		get
		{
			return TactilePlayerPrefs.GetString("nonceValue", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("nonceValue", value);
		}
	}

	private static int PersistedServerTime
	{
		get
		{
			return TactilePlayerPrefs.GetInt("CloudInterfaceBaseServerTime", 0);
		}
		set
		{
			TactilePlayerPrefs.SetInt("CloudInterfaceBaseServerTime", value);
		}
	}

	private static int PersistedServerTimeReceived
	{
		get
		{
			return TactilePlayerPrefs.GetInt("CloudInterfaceBaseServerTimeReceived", 0);
		}
		set
		{
			TactilePlayerPrefs.SetInt("CloudInterfaceBaseServerTimeReceived", value);
		}
	}

	private RequestPriorityQueue requestPriorityQueue;

	private const string PREFS_USER_AUTH_SECRET = "userAuthSecret";

	private const string PREFS_NONCE_SESSION = "nonceSession";

	private const string PREFS_NONCE_VALUE = "nonceValue";

	private const string PREFS_SERVER_TIME = "CloudInterfaceBaseServerTime";

	private const string PREFS_SERVER_TIME_RECEIVED = "CloudInterfaceBaseServerTimeReceived";

	private string host;

	private string packageName;

	private int versionCode;

	private string versionName;

	private string sharedGameSecret;

	private byte[] sharedSecret;

	private byte[] sharedSecretPart1Device;

	private byte[] sharedSecretPart1User;

	private byte[] sharedSecretPart3Device;

	private byte[] sharedSecretPart3User;

	private string deviceUid;

	private readonly IRequestMetaDataProviderRegistry requestMetaDataProviderRegistry;

	private string userAuthSecret;

	private string nonceSession;

	private string nonceValue;

	private int serverTime;

	private int serverTimeReceived;

	private bool requestRunning;

	public enum AuthMode
	{
		None,
		Device,
		User
	}
}
