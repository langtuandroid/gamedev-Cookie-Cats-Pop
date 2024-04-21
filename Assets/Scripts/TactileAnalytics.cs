using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Analytics;
using TactileModules.Analytics.CollectorLoadBalancing;
using TactileModules.Analytics.EventVerification;
using TactileModules.Analytics.Interfaces;
using UnityEngine;

public class TactileAnalytics : SingleInstance<TactileAnalytics>, IAnalytics
{
	protected TactileAnalytics(string appId, string versionName, int versionCode, string userId, TactileAnalytics.Config config)
	{
		this.UpgradePreEventStorePlayerPrefs();
		this.appId = appId;
		this.versionName = versionName;
		this.versionCode = versionCode;
		this.userId = userId;
		if (this.PersistedLastUserId != userId && !this.PersistedIsFirstSession)
		{
			this.PersistedIsFirstSession = true;
		}
		string persistedLastUserId = this.PersistedLastUserId;
		this.PersistedLastUserId = userId;
		this.production = new EventsUploader(TactileAnalytics.productionEnvironmentName, this.appId, this.userId, this.defaultCollectUrlsProduction);
		this.staging = new EventsUploader(TactileAnalytics.stagingEnvironmentName, this.appId, this.userId, this.defaultCollectUrlsStaging);
		this.UpgradePersistedEventStorage();
		this.ConfigurationUpdated(config);
		this.CreateEventVerificationSystems();
		this.platform = SystemInfoHelper.DeviceType.ToUpper();
		ActivityManager.onResumeEvent += this.ApplicationWillEnterForeground;
		this.decoratorRegistry = new DecoratorRegistry();
		this.NewSession();
		this.LogDefaultEvents(true, persistedLastUserId);
		UnityEngine.Object.DontDestroyOnLoad(new GameObject("TactileAnalyticsLifecycle", new Type[]
		{
			typeof(TactileAnalytics.TactileAnalyticsLifecycle)
		}));
	}

	internal EventsUploader Uploader
	{
		get
		{
			return this.production;
		}
	}

	public static void CreateInstance(string appId, string versionName, int versionCode, string userId, TactileAnalytics.Config config)
	{
		new TactileAnalytics(appId, versionName, versionCode, userId, config);
	}

	public static TactileAnalytics Instance
	{
		get
		{
			return SingleInstance<TactileAnalytics>.instance;
		}
	}

	private static void AddValidParams(List<string> names, string prefix, int count)
	{
		for (int i = 1; i <= count; i++)
		{
			names.Add(prefix + i.ToString());
		}
	}

	public static List<string> GetValidDefaultEventParamNames()
	{
		return TactileAnalytics.validDefaultEventParamNames;
	}

	public static List<string> GetValidEventParamNames()
	{
		return TactileAnalytics.validEventParamNames;
	}

	private static List<string> BuildValidDefaultEventParamNames()
	{
		return new List<string>
		{
			"eventName",
			"eventTimestamp",
			"userId",
			"sessionId",
			"platform",
			"versionName",
			"versionCode"
		};
	}

	private static List<string> BuildValidEventParamNames()
	{
		List<string> list = new List<string>();
		TactileAnalytics.AddValidParams(list, "i_param", 100);
		TactileAnalytics.AddValidParams(list, "f_param", 50);
		TactileAnalytics.AddValidParams(list, "b_param", 50);
		TactileAnalytics.AddValidParams(list, "ts_param", 50);
		TactileAnalytics.AddValidParams(list, "s_param", 100);
		return list;
	}

	private void UpgradePersistedEventStorage()
	{
		this.UpgradePreEventStorePlayerPrefs();
		this.UpgradePreEventStoreFiles();
	}

	private void CreateCollectorLoadBalancersRegistry()
	{
		this.collectorLoadBalancers = new CollectorLoadBalancers();
	}

	private void CreateEventVerificationSystems()
	{
		this.CreateCollectorLoadBalancersRegistry();
		this.productionEventCounter = this.CreateEventVerificationSystem(TactileAnalytics.productionEnvironmentName, this.defaultCountUrlsProduction);
		if (this.SendToStagingEnvironment)
		{
			this.stagingEventCounter = this.CreateEventVerificationSystem(TactileAnalytics.stagingEnvironmentName, this.defaultCountUrlsStaging);
		}
	}

	private EventCountLogger CreateEventVerificationSystem(string environmentsName, List<string> collectorUrls)
	{
		CollectorLoadBalancer collectorLoadBalancer = new CollectorLoadBalancer(this.userId, collectorUrls);
		this.collectorLoadBalancers.Add(collectorLoadBalancer);
		ServicePath servicePath = new ServicePath("/count/v1/", this.appId);
		return EventVerificationSystemBuilder.Build(collectorLoadBalancer, servicePath, "CountsStore" + environmentsName, "PackagesStore" + environmentsName, this.userId);
	}

	public void RegisterDecorator(IEventDecorator decorator)
	{
		this.decoratorRegistry.RegisterDecorator(decorator);
	}

	private void UpgradePreEventStorePlayerPrefs()
	{
		if (TactilePlayerPrefs.HasKey("TactileAnalyticsEvents"))
		{
			string securedString = TactilePlayerPrefs.GetSecuredString("TactileAnalyticsEvents", null);
			TactilePlayerPrefs.SetSecuredString("TactileAnalyticsEvents_" + TactileAnalytics.productionEnvironmentName, securedString);
			TactilePlayerPrefs.DeleteKey("TactileAnalyticsEvents");
		}
		if (TactilePlayerPrefs.HasKey("TactileAnalyticsUploadingEvents"))
		{
			string securedString2 = TactilePlayerPrefs.GetSecuredString("TactileAnalyticsUploadingEvents", null);
			TactilePlayerPrefs.SetSecuredString("TactileAnalyticsUploadingEvents_" + TactileAnalytics.productionEnvironmentName, securedString2);
			TactilePlayerPrefs.DeleteKey("TactileAnalyticsUploadingEvents");
		}
		if (TactilePlayerPrefs.HasKey("TactileAnalyticsEventsStaging"))
		{
			string securedString3 = TactilePlayerPrefs.GetSecuredString("TactileAnalyticsEventsStaging", null);
			TactilePlayerPrefs.SetSecuredString("TactileAnalyticsEvents_" + TactileAnalytics.stagingEnvironmentName, securedString3);
			TactilePlayerPrefs.DeleteKey("TactileAnalyticsEventsStaging");
		}
		if (TactilePlayerPrefs.HasKey("TactileAnalyticsUploadingEventsStaging"))
		{
			string securedString4 = TactilePlayerPrefs.GetSecuredString("TactileAnalyticsUploadingEventsStaging", null);
			TactilePlayerPrefs.SetSecuredString("TactileAnalyticsUploadingEvents_" + TactileAnalytics.stagingEnvironmentName, securedString4);
			TactilePlayerPrefs.DeleteKey("TactileAnalyticsUploadingEventsStaging");
		}
	}

	private void UpgradePreEventStoreFiles()
	{
		TactileAnalytics.CopyEventsInPlayerPrefsToEventStore("TactileAnalyticsEvents_" + TactileAnalytics.productionEnvironmentName, this.production);
		TactileAnalytics.CopyEventsInPlayerPrefsToEventStore("TactileAnalyticsEvents_" + TactileAnalytics.stagingEnvironmentName, this.staging);
	}

	internal static void CopyEventsInPlayerPrefsToEventStore(string playerPrefskey, EventsUploader store)
	{
		string securedString = TactilePlayerPrefs.GetSecuredString(playerPrefskey, null);
		if (string.IsNullOrEmpty(securedString))
		{
			return;
		}
		ArrayList arrayList = securedString.arrayListFromJson();
		IEnumerator enumerator = arrayList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				store.StoreEvent((Hashtable)obj);
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
		TactilePlayerPrefs.DeleteKey(playerPrefskey);
	}

	private void ApplicationWillEnterForeground()
	{
		this.collectorLoadBalancers.Reset();
		this.production.ResetCollectUrl(this.userId);
		this.staging.ResetCollectUrl(this.userId);
		this.NewSession();
		this.LogDefaultEvents(false, null);
	}

	public void ConfigurationUpdated(TactileAnalytics.Config config)
	{
		this.sendToStagingModulo = config.SendToStagingModulo;
		this.production.MaxEventListLength = config.MaxEventListLength;
		this.staging.MaxEventListLength = config.MaxEventListLength;
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			this.PersistedLastActiveTimestamp = TactileAnalytics.GetCurrentTimestamp();
		}
	}

	private void LogDefaultEvents(bool coldBooted, string lastUserId = null)
	{
		double persistedLastActiveTimestamp = this.PersistedLastActiveTimestamp;
		string persistedLastSessionId = this.PersistedLastSessionId;
		if (persistedLastActiveTimestamp > 0.0 && persistedLastSessionId != null)
		{
			this.LogEvent(new TactileAnalytics.GameEndedEvent(), persistedLastActiveTimestamp, persistedLastSessionId);
		}
		
		bool persistedIsFirstSession = this.PersistedIsFirstSession;
		if (this.PersistedIsFirstSession)
		{
			if (PlayerPrefs.GetInt("DDSDK_FIRST_RUN", 1) > 0 || this.userId != PlayerPrefs.GetString("DDSDK_USER_ID"))
			{
				this.LogEvent(new TactileAnalytics.NewPlayerEvent(), -1.0, null);
			}
			else
			{
				this.LogEvent(new TactileAnalytics.NewPlayerAlreadyRegisteredDDNAEvent(PlayerPrefs.GetInt("DDSDK_FIRST_RUN", 1), PlayerPrefs.GetString("DDSDK_USER_ID")), -1.0, null);
			}
			if (!string.IsNullOrEmpty(lastUserId) && lastUserId != this.userId)
			{
				this.LogEvent(new TactileAnalytics.UserIdChangedEvent(lastUserId), -1.0, null);
			}
			this.PersistedIsFirstSession = false;
			TactilePlayerPrefs.Save();
		}
		this.LogEvent(new TactileAnalytics.GameStartedEvent(persistedIsFirstSession, coldBooted), -1.0, null);
		this.LogEvent(new TactileAnalytics.ClientDeviceEvent(), -1.0, null);
		this.PersistedLastActiveTimestamp = TactileAnalytics.GetCurrentTimestamp();
		this.PersistedLastSessionId = this.sessionId;
		FiberCtrl.Pool.Run(this.UploadEvents(), true);
	}

	private void NewSession()
	{
		this.sessionId = Guid.NewGuid().ToString();
	}

	public static double DateTimeToUnixEpoch(DateTime dt)
	{
		return (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000.0;
	}

	private static double GetCurrentTimestamp()
	{
		return TactileAnalytics.DateTimeToUnixEpoch(DateTime.UtcNow);
	}

	public void LogEvent(object eventObject, double overrideEventTimestamp = -1.0, string overrideSessionId = null)
	{
		this.decoratorRegistry.InvokeDecorators(eventObject);
		this.PersistedLastActiveTimestamp = TactileAnalytics.GetCurrentTimestamp();
		Type type = eventObject.GetType();
		EventDefinition eventDefinition = TactileAnalytics.eventDefinitions.GetEventDefinition(type);
		TactileAnalytics.EventAttribute eventAttribute = eventDefinition.EventAttribute;
		double num = (overrideEventTimestamp != -1.0) ? overrideEventTimestamp : TactileAnalytics.GetCurrentTimestamp();
		this.LogEventCount(eventDefinition, eventAttribute, num);
		Hashtable hashtable = new Hashtable
		{
			{
				"eventName",
				eventAttribute.EventName
			},
			{
				"eventTimestamp",
				num
			},
			{
				"userId",
				this.userId
			},
			{
				"sessionId",
				(overrideSessionId != null) ? overrideSessionId : this.sessionId
			},
			{
				"platform",
				this.platform
			},
			{
				"versionName",
				this.versionName
			},
			{
				"versionCode",
				this.versionCode
			}
		};
		if (TactileAnalytics.validDefaultEventParamNames.Count != hashtable.Keys.Count)
		{
			throw new Exception(string.Concat(new object[]
			{
				"Unexpected number of default event parameter names. Expected=",
				TactileAnalytics.validDefaultEventParamNames.Count,
				", Found=",
				hashtable.Keys.Count
			}));
		}
		IEnumerator enumerator = hashtable.Keys.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				string text = (string)obj;
				if (TactileAnalytics.validDefaultEventParamNames.IndexOf(text) == -1)
				{
					throw new Exception("Unexpected default event parameter name. DefaultEventParamName=" + text);
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
		foreach (EventParamDefinition eventParamDefinition in eventDefinition.EventParams)
		{
			object obj2 = eventParamDefinition.GetValue(eventObject);
			if (eventParamDefinition.Required && obj2 == null && type != typeof(TactileAnalytics.RequiredParamMissing))
			{
				this.LogEvent(new TactileAnalytics.RequiredParamMissing(eventAttribute.EventName, eventParamDefinition.MemberName, eventObject.GetType().ToString()), -1.0, null);
			}
			if (obj2 != null)
			{
				if (obj2.GetType() == typeof(DateTime))
				{
					obj2 = TactileAnalytics.DateTimeToUnixEpoch((DateTime)obj2);
				}
				hashtable.Add(eventParamDefinition.ParamName, obj2);
			}
		}
		hashtable["eventSchemaHash"] = eventDefinition.SchemaHash;
		FiberCtrl.Pool.Run(this.StoreAndUploadAsync(hashtable), true);
	}

	private void LogEventCount(EventDefinition eventDefinition, TactileAnalytics.EventAttribute eventAttribute, double eventTimestamp)
	{
		this.productionEventCounter.LogEvent(eventDefinition.SchemaHash, eventAttribute.EventName, eventTimestamp);
		if (this.SendToStagingEnvironment)
		{
			this.stagingEventCounter.LogEvent(eventDefinition.SchemaHash, eventAttribute.EventName, eventTimestamp);
		}
	}

	private IEnumerator StoreAndUploadAsync(Hashtable eventData)
	{
		yield return this.production.DoStoreEvent(eventData);
		if (this.SendToStagingEnvironment)
		{
			yield return this.staging.DoStoreEvent(eventData);
		}
		yield return null;
		yield return this.UploadEvents();
		yield break;
	}

	private IEnumerator UploadEvents()
	{
		yield return FiberHelper.RunParallel(new IEnumerator[]
		{
			this.production.UploadEvents(),
			this.staging.UploadEvents()
		});
		yield break;
	}

	private bool SendToStagingEnvironment
	{
		get
		{
			return this.sendToStagingModulo > 0 && Math.Abs(this.userId.GetHashCode()) % this.sendToStagingModulo == 0;
		}
	}

	private bool PersistedIsFirstSession
	{
		get
		{
			return TactilePlayerPrefs.GetBool("TactileAnalyticsIsFirstSession", true);
		}
		set
		{
			TactilePlayerPrefs.SetBool("TactileAnalyticsIsFirstSession", value);
		}
	}

	private double PersistedLastActiveTimestamp
	{
		get
		{
			return double.Parse(TactilePlayerPrefs.GetString("TactileAnalyticsLastActiveTimestamp", "0"));
		}
		set
		{
			TactilePlayerPrefs.SetString("TactileAnalyticsLastActiveTimestamp", value.ToString());
		}
	}

	private string PersistedLastUserId
	{
		get
		{
			return TactilePlayerPrefs.GetString("TactileAnalyticsLastUserId");
		}
		set
		{
			TactilePlayerPrefs.SetString("TactileAnalyticsLastUserId", value);
		}
	}

	private string PersistedLastSessionId
	{
		get
		{
			return TactilePlayerPrefs.GetString("TactileAnalyticsLastSessionId");
		}
		set
		{
			TactilePlayerPrefs.SetString("TactileAnalyticsLastSessionId", value);
		}
	}

	private List<string> defaultCollectUrlsProduction = new List<string>
	{
		"https://analytics.tactilews.com/collect/v3/",
		"https://analytics-eu.tactilews.com/collect/v3/"
	};

	private List<string> defaultCollectUrlsStaging = new List<string>
	{
		"https://analytics-staging.tactilews.com/collect/v3/"
	};

	private List<string> defaultCountUrlsProduction = new List<string>
	{
		"https://analytics.tactilews.com",
		"https://analytics-eu.tactilews.com"
	};

	private List<string> defaultCountUrlsStaging = new List<string>
	{
		"https://analytics-staging.tactilews.com"
	};

	private static string productionEnvironmentName = "production";

	private static string stagingEnvironmentName = "staging";

	private EventsUploader production;

	private EventsUploader staging;

	private string appId;

	private string versionName;

	private int versionCode;

	private string userId;

	private int sendToStagingModulo;

	private string sessionId;

	private string platform;

	private readonly DecoratorRegistry decoratorRegistry;

	private CollectorLoadBalancers collectorLoadBalancers;

	private EventCountLogger productionEventCounter;

	private EventCountLogger stagingEventCounter;

	private static List<string> validDefaultEventParamNames = TactileAnalytics.BuildValidDefaultEventParamNames();

	private static List<string> validEventParamNames = TactileAnalytics.BuildValidEventParamNames();

	private static EventDefinitions eventDefinitions = new EventDefinitions();

	private class TactileAnalyticsLifecycle : MonoBehaviour
	{
		private void OnApplicationPause(bool pauseStatus)
		{
			TactileAnalytics.Instance.OnApplicationPause(pauseStatus);
		}
	}

	[ConfigProvider("AnalyticsConfig")]
	[ObsoleteJsonName(new string[]
	{
		"CollectUrlsProduction",
		"CollectUrlsStaging",
		"ProductionUrls",
		"StagingUrls"
	})]
	public class Config
	{
		[JsonSerializable("SendToStagingModulo", null)]
		public int SendToStagingModulo { get; set; }

		[JsonSerializable("MaxEventListLength", null)]
		public int MaxEventListLength { get; set; }
	}

	public class EventAttribute : Attribute
	{
		public EventAttribute(string eventName, bool validationRequired = true)
		{
			this.EventName = eventName;
			this.ValidationRequired = validationRequired;
		}

		public string EventName { get; private set; }

		public bool ValidationRequired { get; private set; }
	}

	public struct RequiredParam<T>
	{
		public RequiredParam(T value)
		{
			this.paramValue = value;
		}

		public object GetValue()
		{
			return this.paramValue;
		}

		public static implicit operator TactileAnalytics.RequiredParam<T>(T value)
		{
			return new TactileAnalytics.RequiredParam<T>(value);
		}

		public string ToString()
		{
			return this.paramValue.ToString();
		}

		private readonly object paramValue;
	}

	public struct OptionalParam<T>
	{
		public OptionalParam(T value)
		{
			this.paramValue = value;
		}

		public object GetValue()
		{
			return this.paramValue;
		}

		public static implicit operator TactileAnalytics.OptionalParam<T>(T value)
		{
			return new TactileAnalytics.OptionalParam<T>(value);
		}

		public string ToString()
		{
			return this.paramValue.ToString();
		}

		private readonly object paramValue;
	}

	[TactileAnalytics.EventAttribute("newPlayer", true)]
	private class NewPlayerEvent
	{
	}

	[TactileAnalytics.EventAttribute("newPlayerAlreadyRegisteredDDNA", true)]
	private class NewPlayerAlreadyRegisteredDDNAEvent
	{
		public NewPlayerAlreadyRegisteredDDNAEvent(int ddnaSdkFirstRun, string ddnaSdkUserId)
		{
			this.DDNASdkFirstRun = ddnaSdkFirstRun;
			this.DDNASdkUserId = ddnaSdkUserId;
		}

		private TactileAnalytics.RequiredParam<int> DDNASdkFirstRun { get; set; }

		private TactileAnalytics.RequiredParam<string> DDNASdkUserId { get; set; }
	}

	[TactileAnalytics.EventAttribute("userIdChanged", true)]
	private class UserIdChangedEvent
	{
		public UserIdChangedEvent(string lastUserId)
		{
			this.LastUserId = lastUserId;
		}

		private TactileAnalytics.RequiredParam<string> LastUserId { get; set; }
	}

	[TactileAnalytics.EventAttribute("gameStarted", true)]
	private class GameStartedEvent
	{
		public GameStartedEvent(bool isFirstSession, bool coldBooted)
		{
			this.IsFirstSession = isFirstSession;
			this.ColdBooted = coldBooted;
			this.PackageName = SystemInfoHelper.BundleIdentifier;
		}

		private TactileAnalytics.RequiredParam<string> PackageName { get; set; }

		private TactileAnalytics.RequiredParam<bool> IsFirstSession { get; set; }

		private TactileAnalytics.RequiredParam<bool> ColdBooted { get; set; }
	}

	[TactileAnalytics.EventAttribute("gameCrashedLastSession", true)]
	private class GameCrashedLastSessionEvent
	{
		public GameCrashedLastSessionEvent(string crashedSessionId)
		{
			this.CrashedSessionId = crashedSessionId;
		}

		private TactileAnalytics.RequiredParam<string> CrashedSessionId { get; set; }
	}

	[TactileAnalytics.EventAttribute("gameEnded", true)]
	private class GameEndedEvent
	{
	}

	[TactileAnalytics.EventAttribute("clientDevice", true)]
	private class ClientDeviceEvent
	{
		public ClientDeviceEvent()
		{
			this.DeviceName = ClientDeviceEventUtils.GetDeviceName();
			this.DeviceType = ClientDeviceEventUtils.GetDeviceType();
			this.DeviceModel = SystemInfo.deviceModel;
			this.OperatingSystem = ClientDeviceEventUtils.GetOperatingSystem();
			this.OperatingSystemVersion = ClientDeviceEventUtils.GetOperatingSystemVersion();
			this.OperatingSystemUnity = SystemInfo.operatingSystem;
			this.Manufacturer = SystemInfoHelper.Manufacturer;
			this.TimezoneOffset = ClientDeviceEventUtils.GetTimezoneOffset();
			this.DeviceLanguageCode = ClientDeviceEventUtils.GetDeviceLanguageCode();
			this.AdvertisingId = SystemInfoHelper.AdvertisingId;
			this.AdTrackingEnabled = SystemInfoHelper.AdTrackingEnabled;
			if (!string.IsNullOrEmpty(SystemInfoHelper.IFV))
			{
				this.IdentifierForVendor = SystemInfoHelper.IFV;
			}
			this.AvailableInternalStorage = (double)ActivityAndroid.getAvailableInternalStorageSize();
			this.TotalInternalStorage = (double)ActivityAndroid.getTotalInternalStorageSize();
			this.AvailableExternalStorage = (double)ActivityAndroid.getAvailableExternalStorageSize();
			this.TotalExternalStorage = (double)ActivityAndroid.getTotalExternalStorageSize();
		}

		private TactileAnalytics.RequiredParam<string> DeviceName { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> DeviceType { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> DeviceModel { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> OperatingSystem { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> OperatingSystemVersion { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> OperatingSystemUnity { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> Manufacturer { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> TimezoneOffset { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> DeviceLanguageCode { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<string> AdvertisingId { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<bool> AdTrackingEnabled { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<string> IdentifierForVendor { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<double> AvailableInternalStorage { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<double> TotalInternalStorage { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<double> AvailableExternalStorage { [UsedImplicitly] get; set; }

		private TactileAnalytics.OptionalParam<double> TotalExternalStorage { [UsedImplicitly] get; set; }
	}

	[TactileAnalytics.EventAttribute("requiredParamMissing", true)]
	private class RequiredParamMissing
	{
		public RequiredParamMissing(string eventName, string memberName, string eventType)
		{
			this.MissingEventName = eventName;
			this.MissingParamName = memberName;
			this.MissingEventType = eventType;
		}

		private TactileAnalytics.RequiredParam<string> MissingEventName { get; set; }

		private TactileAnalytics.RequiredParam<string> MissingParamName { get; set; }

		private TactileAnalytics.RequiredParam<string> MissingEventType { get; set; }
	}
}
