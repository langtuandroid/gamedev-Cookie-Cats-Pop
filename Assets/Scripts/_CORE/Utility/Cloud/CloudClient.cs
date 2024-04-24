using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Cloud;
using Tactile;
using TactileModules.Foundation;
using TactileModules.TactileCloud;

public class CloudClient : CloudClientBase, ICloudUserSettingsProvider
{
	public CloudClient(IRequestMetaDataProviderRegistry requestMetaDataProviderRegistry) : base(CloudClient.CreateTactileCloudInterface(CloudClientBase.PersistedOverrideCloudServer, requestMetaDataProviderRegistry))
	{
		this.cachedFriends = CloudClient.PersistedCachedFriends;
		this.cachedFriendsSettings = CloudClient.PersistedCachedFriendsSettings;
	}
	

	~CloudClient()
	{
		
	}

	public override IEnumerator UpdateRegistrationCr()
	{
		if (false)
		{
			List<string> friendIds = new List<string>();
			friendIds.Sort();
			Hashtable userData = new Hashtable
			{
				{
					"user",
					new Hashtable
					{
						
					}
				}
			};
			IEnumerator e = base.UpdateRegistrationCr(userData, this.GetUserDataHash(userData));
			while (e.MoveNext())
			{
				object obj = e.Current;
				yield return obj;
			}
		}
		else
		{
			IEnumerator e2 = base.UpdateRegistrationCr(null, null);
			while (e2.MoveNext())
			{
				object obj2 = e2.Current;
				yield return obj2;
			}
		}
		if (base.CachedMe != null)
		{
			IEnumerator e3 = this.UpdateCachedFriendsAndSettings(delegate(object err)
			{
				if (err != null)
				{
				}
			});
			while (e3.MoveNext())
			{
				object obj3 = e3.Current;
				yield return obj3;
			}
		}
		yield break;
	}

	protected override string OneSignalPlayerId { get; }

	public IEnumerator UpdateCachedFriendsAndSettings(Action<object> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!");
			yield break;
		}
		CloudInterface.FriendsAndUserSettingsResponse response = new CloudInterface.FriendsAndUserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.GetFriendsAndUserSettings(base.CachedMe.CloudId, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			this.cachedFriends = new List<CloudUser>(response.Friends);
			this.cachedFriendsSettings = new List<CloudUserSettings>(response.Settings);
			CloudClient.PersistedCachedFriends = this.cachedFriends;
			CloudClient.PersistedCachedFriendsSettings = this.cachedFriendsSettings;
			this.OnFriendsAndSettingsSynced();
			callback(null);
		}
		else
		{
			callback("Could not update cached friends and settings: " + response.ErrorInfo);
		}
		yield break;
	}

	public IEnumerator GetConfiguration(int version, string woogaId, Action<object, Hashtable> callback)
	{
		if (base.CachedDevice == null)
		{
			callback("No valid cloud device!", null);
			yield break;
		}
		ConfigurationResponse response = new ConfigurationResponse();
		IEnumerator e = this.CloudInterfaceInstance.GetConfiguration(version, woogaId, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, (response.ReturnCode != ReturnCode.NoError) ? null : response.Configuration);
		}
		else
		{
			callback("Could not get latest configuration: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator GetOtherUserSettings(string otherUserCloudId, Action<object, CloudUserSettings> callback)
	{
		if (base.CachedDevice == null)
		{
			callback("No valid cloud device!", null);
			yield break;
		}
		CloudInterface.UserSettingsResponse response = new CloudInterface.UserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.GetOtherUserSettings(otherUserCloudId, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			if (response.UserSettings != null)
			{
			}
			callback(null, response.UserSettings);
		}
		else
		{
			callback("Could not get user settings: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator GetUserSettings(Action<object, CloudUserSettings> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null);
			yield break;
		}
		CloudInterface.UserSettingsResponse response = new CloudInterface.UserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.GetUserSettings(base.CachedMe.CloudId, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			if (response.UserSettings != null)
			{
			}
			callback(null, response.UserSettings);
		}
		else
		{
			callback("Could not get user settings: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator CreateUserSettings(Hashtable privateSettings, Hashtable publicSettings, Action<object, CloudUserSettings> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null);
			yield break;
		}
		CloudInterface.UserSettingsResponse response = new CloudInterface.UserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.CreateUserSettings(base.CachedMe.CloudId, privateSettings, publicSettings, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.UserSettings);
		}
		else
		{
			callback("Could not create user settings: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator UpdateUserSettings(Hashtable privateSettings, Hashtable publicSettings, int version, Action<object, CloudUserSettings, bool> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null, false);
			yield break;
		}
		CloudInterface.UserSettingsResponse response = new CloudInterface.UserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.UpdateUserSettings(base.CachedMe.CloudId, privateSettings, publicSettings, version, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.UserSettings, true);
		}
		else if (response.ReturnCode == ReturnCode.NotLatestVersion)
		{
			callback(null, response.UserSettings, false);
		}
		else
		{
			callback("Could not create user settings: " + response.ErrorInfo, null, false);
		}
		yield break;
	}

	public IEnumerator PatchUserSettings(Hashtable objPathsToSet, Hashtable objPathsToUnset, int version, Action<object, CloudUserSettings, bool> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null, false);
			yield break;
		}
		CloudInterface.UserSettingsResponse response = new CloudInterface.UserSettingsResponse();
		IEnumerator e = this.CloudInterfaceInstance.PatchUserSettings(base.CachedMe.CloudId, objPathsToSet, objPathsToUnset, version, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.UserSettings, true);
		}
		else if (response.ReturnCode == ReturnCode.NotLatestVersion)
		{
			callback(null, response.UserSettings, false);
		}
		else
		{
			callback("Could not patch user settings: " + response.ErrorInfo, null, false);
		}
		yield break;
	}

	public IEnumerator SubmitScore(int score, int leaderboard, int videoId, Action<object, CloudScore> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null);
			yield break;
		}
		CloudInterface.LeaderboardsSubmitScoreResponse response = new CloudInterface.LeaderboardsSubmitScoreResponse();
		IEnumerator e = this.CloudInterfaceInstance.LeaderboardsSubmitScore(base.CachedMe.CloudId, score, leaderboard, videoId, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.Score);
		}
		else
		{
			callback("Failed to submit score to server: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator GetScores(int leaderboard, Action<object, Dictionary<string, List<CloudScore>>> callback)
	{
		if (!base.HasValidUser)
		{
			callback("No valid cloud user!", null);
			yield break;
		}
		CloudInterface.LeaderboardsGetScoresResponse response = new CloudInterface.LeaderboardsGetScoresResponse();
		IEnumerator e = this.CloudInterfaceInstance.LeaderboardsGetScores(base.CachedMe.CloudId, leaderboard, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.Scores);
		}
		else
		{
			callback("Failed to receive scores from server: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator TournamentStatus(CloudClient.TournamentResultDelegate callback)
	{
		if (!base.HasValidDevice)
		{
			callback("No valid cloud device!", null, null, 0, 0, 0, 0, DateTime.UtcNow);
			yield break;
		}
		CloudInterface.TournamentResponse response = new CloudInterface.TournamentResponse();
		yield return this.CloudInterfaceInstance.TournamentStatus((!base.HasValidUser) ? null : base.CachedMe.CloudId, response);
		if (response.Success)
		{
			callback(null, response.Entries, response.Users, response.PeriodId, response.TournamentId, response.TournamentEndsInSeconds, response.TournamentClosesInSeconds, response.utcReceived);
		}
		else
		{
			callback("Failed to receive tournament status: " + response.ErrorInfo, null, null, 0, 0, 0, 0, DateTime.UtcNow);
		}
		yield break;
	}

	public IEnumerator TournamentJoin(TournamentCloudManager.Type tournamentType, CloudClient.TournamentResultDelegate callback)
	{
		if (!base.HasValidDevice)
		{
			callback("No valid cloud device!", null, null, 0, 0, 0, 0, DateTime.UtcNow);
			yield break;
		}
		CloudInterface.TournamentResponse response = new CloudInterface.TournamentResponse();
		yield return this.CloudInterfaceInstance.TournamentJoin((!base.HasValidUser) ? null : base.CachedMe.CloudId, tournamentType, response);
		if (response.Success)
		{
			callback(null, response.Entries, response.Users, response.PeriodId, response.TournamentId, response.TournamentEndsInSeconds, response.TournamentClosesInSeconds, response.utcReceived);
		}
		else
		{
			callback("Failed to join tournament: " + response.ErrorInfo, null, null, 0, 0, 0, 0, DateTime.UtcNow);
		}
		yield break;
	}

	public IEnumerator TournamentSubmitScore(int periodId, int tournamentId, int leaderboard, int score, CloudClient.TournamentResultDelegate callback)
	{
		if (!base.HasValidDevice)
		{
			callback("No valid cloud device!", null, null, 0, 0, 0, 0, DateTime.UtcNow);
			yield break;
		}
		CloudInterface.TournamentResponse response = new CloudInterface.TournamentResponse();
		yield return this.CloudInterfaceInstance.TournamentSubmitScore((!base.HasValidUser) ? null : base.CachedMe.CloudId, periodId, tournamentId, leaderboard, score, response);
		if (response.Success)
		{
			callback(null, response.Entries, response.Users, response.PeriodId, response.TournamentId, response.TournamentEndsInSeconds, response.TournamentClosesInSeconds, response.utcReceived);
		}
		else
		{
			callback("Failed to submit score to server: " + response.ErrorInfo, null, null, 0, 0, 0, 0, DateTime.UtcNow);
		}
		yield break;
	}

	public IEnumerator TournamentGetEntries(int periodId, int tournamentId, CloudClient.TournamentResultDelegate callback)
	{
		if (!base.HasValidDevice)
		{
			callback("No valid cloud device!", null, null, 0, 0, 0, 0, DateTime.UtcNow);
			yield break;
		}
		CloudInterface.TournamentResponse response = new CloudInterface.TournamentResponse();
		yield return this.CloudInterfaceInstance.TournamentGetEntries((!base.HasValidUser) ? null : base.CachedMe.CloudId, periodId, tournamentId, response);
		if (response.Success)
		{
			callback(null, response.Entries, response.Users, response.PeriodId, response.TournamentId, response.TournamentEndsInSeconds, response.TournamentClosesInSeconds, response.utcReceived);
		}
		else
		{
			callback("Failed to receive tournament entries from server: " + response.ErrorInfo, null, null, 0, 0, 0, 0, DateTime.UtcNow);
		}
		yield break;
	}

	public IEnumerator TournamentPresent(int periodId, int tournamentId, CloudClient.TournamentPresentResultDelegate callback)
	{
		if (!base.HasValidDevice)
		{
			callback("No valid cloud device!", null, null, 0, 0, 0, 0, DateTime.UtcNow, TournamentCloudManager.PresentResult.Unknown);
			yield break;
		}
		CloudInterface.TournamentPresentResponse response = new CloudInterface.TournamentPresentResponse();
		yield return this.CloudInterfaceInstance.TournamentPresent((!base.HasValidUser) ? null : base.CachedMe.CloudId, periodId, tournamentId, response);
		if (response.Success)
		{
			callback(null, response.Entries, response.Users, response.PeriodId, response.TournamentId, response.TournamentEndsInSeconds, response.TournamentClosesInSeconds, response.utcReceived, response.PresentResult);
		}
		else
		{
			callback("Failed to present tournament entries: " + response.ErrorInfo, null, null, 0, 0, 0, 0, DateTime.UtcNow, TournamentCloudManager.PresentResult.Unknown);
		}
		yield break;
	}

	public IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Action<object, Hashtable, bool> callback)
	{
		CloudInterface.PurchaseResponse response = new CloudInterface.PurchaseResponse();
		IEnumerator e = this.CloudInterfaceInstance.ReportPurchase(base64EncodedTransactionReceipt, eventParams, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.Receipt, response.IsNew);
		}
		else
		{
			callback("Could not verify receipt: " + response.ErrorInfo, null, false);
		}
		yield break;
	}

	protected override void ResetClient()
	{
		CloudClient.PersistedCachedFriends = null;
		CloudClient.PersistedCachedFriendsSettings = null;
		this.cachedFriends = CloudClient.PersistedCachedFriends;
		this.cachedFriendsSettings = CloudClient.PersistedCachedFriendsSettings;
		base.ResetClient();
	}

	public override void ClearCachedAndPersistedUserData()
	{
		CloudClient.PersistedCachedFriends = null;
		CloudClient.PersistedCachedFriendsSettings = null;
		this.cachedFriends = CloudClient.PersistedCachedFriends;
		this.cachedFriendsSettings = CloudClient.PersistedCachedFriendsSettings;
		base.ClearCachedAndPersistedUserData();
	}

	public List<CloudUser> CachedFriends
	{
		get
		{
			return this.cachedFriends;
		}
	}

	public CloudUser GetUserForCloudId(string userId)
	{
		if (base.CachedMe != null && userId == base.CachedMe.CloudId)
		{
			return base.CachedMe;
		}
		foreach (CloudUser cloudUser in this.cachedFriends)
		{
			if (cloudUser.CloudId == userId)
			{
				return cloudUser;
			}
		}
		return null;
	}

	public CloudUser GetUserForFacebookId(string facebookId)
	{
		if (base.CachedMe != null && facebookId == base.CachedMe.FacebookId)
		{
			return base.CachedMe;
		}
		foreach (CloudUser cloudUser in this.cachedFriends)
		{
			if (facebookId == cloudUser.FacebookId)
			{
				return cloudUser;
			}
		}
		return null;
	}

	public PublicUserSettings GetSettingsForCloudUser(CloudUser user)
	{
		if (user == null)
		{
			return null;
		}
		if (base.CachedMe != null && user.CloudId == base.CachedMe.CloudId)
		{
			return UserSettingsManager.Instance.GetSettings<PublicUserSettings>();
		}
		foreach (CloudUserSettings cloudUserSettings in this.cachedFriendsSettings)
		{
			if (cloudUserSettings.UserId == user.CloudId)
			{
				return UserSettingsManager.Instance.GetFriendSettings<PublicUserSettings>(user);
			}
		}
		return null;
	}

	public CloudUserSettings GetCloudSettingsForCloudUser(CloudUser user)
	{
		if (user == null)
		{
			return null;
		}
		if (base.CachedMe != null && user.CloudId == base.CachedMe.CloudId)
		{
			return null;
		}
		foreach (CloudUserSettings cloudUserSettings in this.cachedFriendsSettings)
		{
			if (cloudUserSettings.UserId == user.CloudId)
			{
				return cloudUserSettings;
			}
		}
		return null;
	}

	private void FacebookLogoutHandler()
	{
		this.ClearCachedAndPersistedUserData();
	}

	private string GetUserDataHash(Hashtable userData)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Hashtable hashtable = (Hashtable)userData["user"];
		stringBuilder.Append(hashtable["facebookId"]);
		stringBuilder.Append(hashtable["facebookAccessToken"]);
		foreach (string value in hashtable["facebookFriendIds"] as string[])
		{
			stringBuilder.Append(value);
		}
		stringBuilder.Append(hashtable["displayName"]);
		SHA1 sha = new SHA1CryptoServiceProvider();
		byte[] array2 = sha.ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		StringBuilder stringBuilder2 = new StringBuilder(array2.Length * 2);
		foreach (byte b in array2)
		{
			stringBuilder2.Append(b.ToString("x2"));
		}
		return stringBuilder2.ToString();
	}

	private static ICloudInterfaceBase CreateTactileCloudInterface(string overrideCloudUrl, IRequestMetaDataProviderRegistry requestMetaDataProviderRegistry)
	{
		string host = Constants.CLOUD_URL;
		if (!string.IsNullOrEmpty(overrideCloudUrl))
		{
			host = overrideCloudUrl;
		}
		string bundleIdentifier = SystemInfoHelper.BundleIdentifier;
		int versionCode = 0;
		int.TryParse(SystemInfoHelper.BundleVersion, out versionCode);
		string bundleShortVersion = SystemInfoHelper.BundleShortVersion;
		return new CloudInterface(host, bundleIdentifier, versionCode, bundleShortVersion, Constants.SHARED_GAME_SECRET, SystemInfoHelper.DeviceID, Constants.APS_ENVIRONMENT, requestMetaDataProviderRegistry);
	}
	

	private static List<CloudUser> PersistedCachedFriends
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("CloudClientCachedCloudFriends", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.ArrayListToGenericList<CloudUser>(securedString.arrayListFromJson());
			}
			return new List<CloudUser>();
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("CloudClientCachedCloudFriends", JsonSerializer.GenericListToArrayList<CloudUser>(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("CloudClientCachedCloudFriends", string.Empty);
			}
		}
	}

	private static List<CloudUserSettings> PersistedCachedFriendsSettings
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("CloudClientCachedCloudFriendsSettings", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.ArrayListToGenericList<CloudUserSettings>(securedString.arrayListFromJson());
			}
			return new List<CloudUserSettings>();
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("CloudClientCachedCloudFriendsSettings", JsonSerializer.GenericListToArrayList<CloudUserSettings>(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("CloudClientCachedCloudFriendsSettings", string.Empty);
			}
		}
	}

	public ICloudInterface CloudInterfaceInstance
	{
		get
		{
			return (ICloudInterface)this.cloudInterface;
		}
	}

	private void OnFriendsAndSettingsSynced()
	{
		if (this.FriendsAndSettingsSynced != null)
		{
			this.FriendsAndSettingsSynced();
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action FriendsAndSettingsSynced;

	private const string PREFS_CACHED_FRIENDS = "CloudClientCachedCloudFriends";

	private const string PREFS_CACHED_FRIENDS_SETTINGS = "CloudClientCachedCloudFriendsSettings";

	private List<CloudUser> cachedFriends;

	private List<CloudUserSettings> cachedFriendsSettings;
	
	public delegate void TournamentResultDelegate(object error, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int endsInSeconds, int closesInSeconds, DateTime utcReceived);

	public delegate void TournamentPresentResultDelegate(object error, List<TournamentCloudManager.Entry> entries, List<CloudUser> users, int periodId, int tournamentId, int endsInSeconds, int closesInSeconds, DateTime utcReceived, TournamentCloudManager.PresentResult presentResult);
}
