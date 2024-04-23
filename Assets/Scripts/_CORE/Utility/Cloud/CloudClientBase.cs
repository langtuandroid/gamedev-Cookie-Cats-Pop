using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cloud;
using Fibers;
using TactileModules.TactileCloud;
using TactileModules.TactileCloud.AssetBundles;

public abstract class CloudClientBase : SingleInstance<CloudClientBase>, ICloudClientState
{
	protected CloudClientBase(ICloudInterfaceBase cloudInterfaceBase)
	{
		this.cloudInterface = cloudInterfaceBase;
		this.cloudInterface.OnServerTimeUpdated += delegate(int time)
		{
			if (this.OnServerTimeUpdated != null)
			{
				this.OnServerTimeUpdated(time);
			}
		};
		if (CloudClientBase.PersistedLastCloudUrl.Length > 0 && CloudClientBase.PersistedLastCloudUrl != this.cloudInterface.Host)
		{
			this.ResetClient();
		}
		CloudClientBase.PersistedLastCloudUrl = this.cloudInterface.Host;
		this.lastSentDeviceDataHash = CloudClientBase.PersistedLastSentDeviceDataHash;
		this.cachedDevice = CloudClientBase.PersistedCachedDevice;
		this.lastSentUserDataHash = CloudClientBase.PersistedLastSentUserDataHash;
		this.cachedMe = CloudClientBase.PersistedCachedMe;
		if (this.cachedMe != null && string.IsNullOrEmpty(this.cachedMe.ExternalId))
		{
			this.ClearCachedAndPersistedUserData();
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<int> OnServerTimeUpdated;

	private void OnUserChanged()
	{
		if (this.UserChanged != null)
		{
			this.UserChanged(this.cachedMe);
		}
	}

	private void OnUserUpdated()
	{
		if (this.UserUpdated != null)
		{
			this.UserUpdated(this.cachedMe);
		}
	}

	protected virtual void ResetClient()
	{
		CloudClientBase.PersistedLastSentDeviceDataHash = string.Empty;
		CloudClientBase.PersistedLastSentUserDataHash = string.Empty;
		CloudClientBase.PersistedCachedDevice = null;
		CloudClientBase.PersistedCachedMe = null;
		this.lastSentDeviceDataHash = string.Empty;
		this.lastSentUserDataHash = string.Empty;
		this.cachedDevice = null;
		this.cachedMe = null;
		this.OnUserChanged();
	}

	public virtual void ClearCachedAndPersistedUserData()
	{
		if (this.HasValidUser)
		{
			FiberCtrl.Pool.Run(this.LogoutUserFromDevice(this.cachedMe.CloudId), false);
		}
		CloudClientBase.PersistedLastSentUserDataHash = string.Empty;
		CloudClientBase.PersistedCachedMe = null;
		this.lastSentUserDataHash = string.Empty;
		this.cachedMe = null;
		this.OnUserChanged();
	}

	private bool IsError(object error)
	{
		return error != null;
	}

	public abstract IEnumerator UpdateRegistrationCr();

	protected IEnumerator UpdateRegistrationCr(Hashtable userData, string userDataHash)
	{
		if (this.updateRegistrationInProgress)
		{
			while (this.updateRegistrationInProgress)
			{
				yield return null;
			}
		}
		else
		{
			yield return new Fiber.OnExit(delegate()
			{
				this.updateRegistrationInProgress = false;
			});
			this.updateRegistrationInProgress = true;
			yield return this.UpdateRegistrationCrInternal(userData, userDataHash);
		}
		yield break;
	}

	private CloudLocalDevice CreateLocalDevice()
	{
		return CloudLocalDevice.Create(this.cachedDevice == null || this.cachedDevice.PushEnabled, this.OneSignalPlayerId);
	}

	private IEnumerator UpdateRegistrationCrInternal(Hashtable userData, string userDataHash)
	{
		object error = null;
		CloudLocalDevice d = this.CreateLocalDevice();
		if (d.hash != this.lastSentDeviceDataHash || this.cachedDevice == null)
		{
			yield return this.CreateOrUpdateDevice(d, delegate(object err)
			{
				error = err;
			});
			if (this.IsError(error))
			{
				yield break;
			}
		}
		if (userData != null && (userDataHash != this.lastSentUserDataHash || this.cachedMe == null))
		{
			yield return this.CreateOrUpdateUser(userData, userDataHash, delegate(object err)
			{
				error = err;
			});
			if (this.IsError(error))
			{
				yield break;
			}
		}
		
		yield break;
	}

	public IEnumerator DeleteCloudUser(Action<object> callback)
	{
		if (!this.HasValidUser)
		{
			callback("No valid cloud user");
			yield break;
		}
		Response response = new Response();
		yield return this.cloudInterface.DeleteCloudUser(this.CachedMe.CloudId, response);
		callback((!response.Success) ? response.ErrorInfo : null);
		yield break;
	}

	public IEnumerator SendPush(string receiverId, string message, Dictionary<string, string> payload, Action<object> callback)
	{
		if (!this.HasValidUser)
		{
			callback("No valid cloud user");
			yield break;
		}
		Response response = new Response();
		yield return this.cloudInterface.SendPush(this.CachedMe.CloudId, receiverId, message, payload, response);
		callback((!response.Success) ? response.ErrorInfo : null);
		yield break;
	}

	private IEnumerator CreateOrUpdateDevice(CloudLocalDevice d, Action<object> callback)
	{
		DeviceResponse response = new DeviceResponse();
		yield return this.cloudInterface.CreateOrUpdateDevice(d, response);
		if (response.Success)
		{
			this.cachedDevice = response.Device;
			CloudClientBase.PersistedCachedDevice = this.cachedDevice;
			this.lastSentDeviceDataHash = d.hash;
			CloudClientBase.PersistedLastSentDeviceDataHash = this.lastSentDeviceDataHash;
			callback(null);
		}
		else
		{
			callback("Could not create/update device: " + response.ErrorInfo);
		}
		yield break;
	}

	private IEnumerator CreateOrUpdateUser(Hashtable userData, string userDataHash, Action<object> callback)
	{
		UserResponse response = new UserResponse();
		yield return this.cloudInterface.CreateOrUpdateUser(userData, response);
		if (response.Success)
		{
			bool flag = this.cachedMe == null || this.cachedMe.CloudId != response.User.CloudId;
			this.cachedMe = response.User;
			this.cloudInterface.UserAuthSecret = response.User.AuthSecret;
			CloudClientBase.PersistedCachedMe = this.cachedMe;
			this.lastSentUserDataHash = userDataHash;
			CloudClientBase.PersistedLastSentUserDataHash = this.lastSentUserDataHash;
			callback(null);
			if (flag)
			{
				this.OnUserChanged();
			}
			else
			{
				this.OnUserUpdated();
			}
		}
		else
		{
			callback("Could not create/update user: " + response.ErrorInfo);
		}
		yield break;
	}

	private IEnumerator LogoutUserFromDevice(string userId)
	{
		Response response = new Response();
		yield return this.cloudInterface.LogoutUserFromDevice(userId, response);
		if (response.Success)
		{
		}
		yield break;
	}

	public IEnumerator GetConfiguration(int version, Action<object, Hashtable> callback)
	{
		if (this.CachedDevice == null)
		{
			callback("No valid cloud device!", null);
			yield break;
		}
		ConfigurationResponse response = new ConfigurationResponse();
		yield return this.cloudInterface.GetConfiguration(version, response);
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

	public IEnumerator GetAssetBundles(int version, Action<object, int, Dictionary<string, AssetBundleInfo>> callback)
	{
		if (this.CachedDevice == null)
		{
			callback("No valid cloud device!", 0, null);
			yield break;
		}
		AssetBundleResponse response = new AssetBundleResponse();
		yield return this.cloudInterface.GetAssetBundles(version, response);
		if (response.Success)
		{
			callback(null, response.Version, (response.ReturnCode != ReturnCode.NoError) ? null : response.AssetBundles);
		}
		else
		{
			callback("Could not get latest asset bundles: " + response.ErrorInfo, 0, null);
		}
		yield break;
	}

	public IEnumerator ReportPurchase(string base64EncodedTransactionReceipt, Dictionary<string, string> eventParams, Action<object, Hashtable, bool, bool> callback)
	{
		PurchaseResponse response = new PurchaseResponse();
		IEnumerator e = this.cloudInterface.ReportPurchase(base64EncodedTransactionReceipt, eventParams, response);
		while (e.MoveNext())
		{
			object obj = e.Current;
			yield return obj;
		}
		if (response.Success)
		{
			callback(null, response.Receipt, response.IsNew, response.IsSandbox);
		}
		else
		{
			callback("Could not verify receipt: " + response.ErrorInfo, null, false, false);
		}
		yield break;
	}

	public IEnumerator AdsGetEnabled(Action<object, List<string>, List<string>> callback)
	{
		AdsEnabledResponse response = new AdsEnabledResponse();
		yield return this.cloudInterface.AdsGetEnabled(response);
		if (response.Success)
		{
			callback(null, response.InterstitialProviders, response.VideoProviders);
		}
		else
		{
			callback("Could not get ad init: " + response.ErrorInfo, null, null);
		}
		yield break;
	}

	public IEnumerator AdsGetPriority(Action<object, Dictionary<string, Dictionary<string, List<string>>>> callback)
	{
		AdsPriorityResponse response = new AdsPriorityResponse();
		yield return this.cloudInterface.AdsGetPriority(response);
		if (response.Success)
		{
			callback(null, response.Priorities);
		}
		else
		{
			callback("Could not get ad priorities: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator AdsReportImpression(string adType, string adLocation, string provider, Action<object, Dictionary<string, Dictionary<string, List<string>>>> callback)
	{
		AdsPriorityResponse response = new AdsPriorityResponse();
		yield return this.cloudInterface.AdsReportImpression(adType, adLocation, provider, response);
		if (response.Success)
		{
			callback(null, response.Priorities);
		}
		else
		{
			callback("Could not report impression: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public IEnumerator AdsReportRequest(string adType, string adLocation, string provider, Action<object> callback)
	{
		Response response = new Response();
		yield return this.cloudInterface.AdsReportRequest(adType, adLocation, provider, response);
		if (response.Success)
		{
			callback(null);
		}
		else
		{
			callback("Could not report request: " + response.ErrorInfo);
		}
		yield break;
	}

	public IEnumerator ReportAdjustIOAttribution(Hashtable attributionData, Action<object> callback)
	{
		Response response = new Response();
		yield return this.cloudInterface.ReportAdjustIOAttribution(attributionData, response);
		if (response.Success)
		{
			callback(null);
		}
		else
		{
			callback("Could not report request: " + response.ErrorInfo);
		}
		yield break;
	}

	public IEnumerator CheckFacebookPayment(long paymentId, Action<object, Hashtable> callback)
	{
		FacebookPaymentResponse response = new FacebookPaymentResponse();
		yield return this.cloudInterface.CheckFacebookPayment(paymentId, response);
		if (response.Success)
		{
			callback(null, response.Payment);
		}
		else
		{
			callback("Failed to check facebook payment: " + response.ErrorInfo, null);
		}
		yield break;
	}

	public void UpdateTactileCloudUrl(string newCloudUrl)
	{
		this.cloudInterface.Host = newCloudUrl;
		this.ResetClient();
		FiberCtrl.Pool.Run(this.UpdateRegistrationCr(), false);
	}

	public bool HasValidDevice
	{
		get
		{
			return this.cachedDevice != null;
		}
	}

	public CloudDevice CachedDevice
	{
		get
		{
			return this.cachedDevice;
		}
	}

	public bool HasValidUser
	{
		get
		{
			return this.cachedMe != null;
		}
	}

	public CloudUser CachedMe
	{
		get
		{
			return this.cachedMe;
		}
	}

	public int LastReceivedServerTimeUnixEpocUTC
	{
		get
		{
			return this.cloudInterface.LastReceivedServerTimeUnixEpocUTC;
		}
	}

	public int ClientAdjustedServerTimeUnixEpocUTC
	{
		get
		{
			return this.cloudInterface.ClientAdjustedServerTimeUnixEpocUTC;
		}
	}

	protected abstract string OneSignalPlayerId { get; }

	private static string PersistedLastCloudUrl
	{
		get
		{
			return TactilePlayerPrefs.GetSecuredString("lastCloudUrl", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetSecuredString("lastCloudUrl", value);
		}
	}

	public static string PersistedOverrideCloudServer
	{
		get
		{
			return TactilePlayerPrefs.GetSecuredString("overrideCloudUrl", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetSecuredString("overrideCloudUrl", value);
		}
	}

	private static string PersistedLastSentDeviceDataHash
	{
		get
		{
			return TactilePlayerPrefs.GetString("lastSentDeviceDataHash", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("lastSentDeviceDataHash", value);
		}
	}

	private static string PersistedLastSentUserDataHash
	{
		get
		{
			return TactilePlayerPrefs.GetString("lastSentUserDataHash", string.Empty);
		}
		set
		{
			TactilePlayerPrefs.SetString("lastSentUserDataHash", value);
		}
	}

	private static CloudDevice PersistedCachedDevice
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("cachedCloudDevice", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.HashtableToObject<CloudDevice>(securedString.hashtableFromJson());
			}
			return null;
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("cachedCloudDevice", JsonSerializer.ObjectToHashtable(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("cachedCloudDevice", string.Empty);
			}
		}
	}

	private static CloudUser PersistedCachedMe
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("cachedCloudUser", string.Empty);
			if (securedString.Length > 0)
			{
				return JsonSerializer.HashtableToObject<CloudUser>(securedString.hashtableFromJson());
			}
			return null;
		}
		set
		{
			if (value != null)
			{
				TactilePlayerPrefs.SetSecuredString("cachedCloudUser", JsonSerializer.ObjectToHashtable(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString("cachedCloudUser", string.Empty);
			}
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<CloudUser> UserUpdated;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<CloudUser> UserChanged;

	private const string PREFS_LAST_CLOUD_URL = "lastCloudUrl";

	private const string PREFS_OVERRIDE_CLOUD_URL = "overrideCloudUrl";

	private const string PREFS_LAST_SENT_DEVICE_DATA_HASH = "lastSentDeviceDataHash";

	private const string PREFS_LAST_SENT_USER_DATA_HASH = "lastSentUserDataHash";

	private const string PREFS_CACHED_DEVICE = "cachedCloudDevice";

	private const string PREFS_CACHED_ME = "cachedCloudUser";

	public readonly ICloudInterfaceBase cloudInterface;

	private string lastSentDeviceDataHash;

	private CloudDevice cachedDevice;

	private string lastSentUserDataHash;

	private CloudUser cachedMe;

	private bool updateRegistrationInProgress;
}
