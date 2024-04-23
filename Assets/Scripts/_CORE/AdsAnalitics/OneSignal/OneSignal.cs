using System;
using System.Collections.Generic;
using OneSignalPush.MiniJSON;
using UnityEngine;

public class OneSignal : MonoBehaviour
{
	public static event OneSignal.PermissionObservable permissionObserver;

	private static void addPermissionObserver()
	{
		if (!OneSignal.addedPermissionObserver && OneSignal.internalPermissionObserver != null && OneSignal.internalPermissionObserver.GetInvocationList().Length > 0)
		{
			OneSignal.addedPermissionObserver = true;
			OneSignal.oneSignalPlatform.addPermissionObserver();
		}
	}

	public static event OneSignal.SubscriptionObservable subscriptionObserver;

	private static void addSubscriptionObserver()
	{
		if (!OneSignal.addedSubscriptionObserver && OneSignal.internalSubscriptionObserver != null && OneSignal.internalSubscriptionObserver.GetInvocationList().Length > 0)
		{
			OneSignal.addedSubscriptionObserver = true;
			OneSignal.oneSignalPlatform.addSubscriptionObserver();
		}
	}

	public static event OneSignal.EmailSubscriptionObservable emailSubscriptionObserver;

	private static void addEmailSubscriptionObserver()
	{
		if (!OneSignal.addedEmailSubscriptionObserver && OneSignal.internalEmailSubscriptionObserver != null && OneSignal.internalEmailSubscriptionObserver.GetInvocationList().Length > 0)
		{
			OneSignal.addedEmailSubscriptionObserver = true;
			OneSignal.oneSignalPlatform.addEmailSubscriptionObserver();
		}
	}

	public static OneSignal.UnityBuilder StartInit(string appID, string googleProjectNumber = null)
	{
		if (OneSignal.builder == null)
		{
			OneSignal.builder = new OneSignal.UnityBuilder();
		}
		OneSignal.builder.appID = appID;
		OneSignal.builder.googleProjectNumber = googleProjectNumber;
		return OneSignal.builder;
	}

	private static void Init()
	{
		OneSignal.initOneSignalPlatform();
	}

	private static void initOneSignalPlatform()
	{
		if (OneSignal.oneSignalPlatform != null || OneSignal.builder == null)
		{
			return;
		}
		OneSignal.initAndroid();
		GameObject gameObject = new GameObject("OneSignalRuntimeObject_KEEP");
		gameObject.AddComponent<OneSignal>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		OneSignal.addPermissionObserver();
		OneSignal.addSubscriptionObserver();
	}

	private static void initAndroid()
	{
		OneSignal.oneSignalPlatform = new OneSignalAndroid("OneSignalRuntimeObject_KEEP", OneSignal.builder.googleProjectNumber, OneSignal.builder.appID, OneSignal.inFocusDisplayType, OneSignal.logLevel, OneSignal.visualLogLevel, OneSignal.requiresUserConsent);
	}

	private static void initUnityEditor()
	{
		MonoBehaviour.print("Please run OneSignal on a device to see push notifications.");
	}

	public static OneSignal.OSInFocusDisplayOption inFocusDisplayType
	{
		get
		{
			return OneSignal._inFocusDisplayType;
		}
		set
		{
			OneSignal._inFocusDisplayType = value;
			if (OneSignal.oneSignalPlatform != null)
			{
				OneSignal.oneSignalPlatform.SetInFocusDisplaying(OneSignal._inFocusDisplayType);
			}
		}
	}

	public static void SetLogLevel(OneSignal.LOG_LEVEL inLogLevel, OneSignal.LOG_LEVEL inVisualLevel)
	{
		OneSignal.logLevel = inLogLevel;
		OneSignal.visualLogLevel = inVisualLevel;
	}

	public static void SetLocationShared(bool shared)
	{
		OneSignal.oneSignalPlatform.SetLocationShared(shared);
	}

	public static void SendTag(string tagName, string tagValue)
	{
		OneSignal.oneSignalPlatform.SendTag(tagName, tagValue);
	}

	public static void SendTags(Dictionary<string, string> tags)
	{
		OneSignal.oneSignalPlatform.SendTags(tags);
	}

	public static void GetTags(OneSignal.TagsReceived inTagsReceivedDelegate)
	{
		OneSignal.tagsReceivedDelegate = inTagsReceivedDelegate;
		OneSignal.oneSignalPlatform.GetTags();
	}

	public static void GetTags()
	{
		OneSignal.oneSignalPlatform.GetTags();
	}

	public static void DeleteTag(string key)
	{
		OneSignal.oneSignalPlatform.DeleteTag(key);
	}

	public static void DeleteTags(IList<string> keys)
	{
		OneSignal.oneSignalPlatform.DeleteTags(keys);
	}

	public static void RegisterForPushNotifications()
	{
		OneSignal.oneSignalPlatform.RegisterForPushNotifications();
	}

	public static void PromptForPushNotificationsWithUserResponse(OneSignal.PromptForPushNotificationsUserResponse inDelegate)
	{
		OneSignal.notificationUserResponseDelegate = inDelegate;
		OneSignal.oneSignalPlatform.promptForPushNotificationsWithUserResponse();
	}

	public static void IdsAvailable(OneSignal.IdsAvailableCallback inIdsAvailableDelegate)
	{
		OneSignal.idsAvailableDelegate = inIdsAvailableDelegate;
		OneSignal.oneSignalPlatform.IdsAvailable();
	}

	public static void IdsAvailable()
	{
		OneSignal.oneSignalPlatform.IdsAvailable();
	}

	public static void EnableVibrate(bool enable)
	{
		((OneSignalAndroid)OneSignal.oneSignalPlatform).EnableVibrate(enable);
	}

	public static void EnableSound(bool enable)
	{
		((OneSignalAndroid)OneSignal.oneSignalPlatform).EnableSound(enable);
	}

	public static void ClearOneSignalNotifications()
	{
		((OneSignalAndroid)OneSignal.oneSignalPlatform).ClearOneSignalNotifications();
	}

	public static void SetSubscription(bool enable)
	{
		OneSignal.oneSignalPlatform.SetSubscription(enable);
	}

	public static void PostNotification(Dictionary<string, object> data)
	{
		OneSignal.PostNotification(data, null, null);
	}

	public static void SetEmail(string email, OneSignal.OnSetEmailSuccess successDelegate, OneSignal.OnSetEmailFailure failureDelegate)
	{
		OneSignal.setEmailSuccessDelegate = successDelegate;
		OneSignal.setEmailFailureDelegate = failureDelegate;
		OneSignal.oneSignalPlatform.SetEmail(email);
	}

	public static void SetEmail(string email, string emailAuthToken, OneSignal.OnSetEmailSuccess successDelegate, OneSignal.OnSetEmailFailure failureDelegate)
	{
		OneSignal.setEmailSuccessDelegate = successDelegate;
		OneSignal.setEmailFailureDelegate = failureDelegate;
		OneSignal.oneSignalPlatform.SetEmail(email, emailAuthToken);
	}

	public static void LogoutEmail(OneSignal.OnLogoutEmailSuccess successDelegate, OneSignal.OnLogoutEmailFailure failureDelegate)
	{
		OneSignal.logoutEmailSuccessDelegate = successDelegate;
		OneSignal.logoutEmailFailureDelegate = failureDelegate;
		OneSignal.oneSignalPlatform.LogoutEmail();
	}

	public static void SetEmail(string email)
	{
		OneSignal.oneSignalPlatform.SetEmail(email);
	}

	public static void SetEmail(string email, string emailAuthToken)
	{
		OneSignal.oneSignalPlatform.SetEmail(email, emailAuthToken);
	}

	public static void LogoutEmail()
	{
		OneSignal.oneSignalPlatform.LogoutEmail();
	}

	public static void PostNotification(Dictionary<string, object> data, OneSignal.OnPostNotificationSuccess inOnPostNotificationSuccess, OneSignal.OnPostNotificationFailure inOnPostNotificationFailure)
	{
		OneSignal.postNotificationSuccessDelegate = inOnPostNotificationSuccess;
		OneSignal.postNotificationFailureDelegate = inOnPostNotificationFailure;
		OneSignal.oneSignalPlatform.PostNotification(data);
	}

	public static void SyncHashedEmail(string email)
	{
		OneSignal.oneSignalPlatform.SyncHashedEmail(email);
	}

	public static void PromptLocation()
	{
		OneSignal.oneSignalPlatform.PromptLocation();
	}

	public static OSPermissionSubscriptionState GetPermissionSubscriptionState()
	{
		return OneSignal.oneSignalPlatform.getPermissionSubscriptionState();
	}

	public static void UserDidProvideConsent(bool consent)
	{
		OneSignal.oneSignalPlatform.UserDidProvideConsent(consent);
	}

	public static bool UserProvidedConsent()
	{
		return OneSignal.oneSignalPlatform.UserProvidedConsent();
	}

	public static void SetRequiresUserPrivacyConsent(bool required)
	{
		OneSignal.requiresUserConsent = required;
	}

	public static void SetExternalUserId(string externalId)
	{
		OneSignal.oneSignalPlatform.SetExternalUserId(externalId);
	}

	public static void RemoveExternalUserId()
	{
		OneSignal.oneSignalPlatform.RemoveExternalUserId();
	}

	private OSNotification DictionaryToNotification(Dictionary<string, object> jsonObject)
	{
		OSNotification osnotification = new OSNotification();
		OSNotificationPayload osnotificationPayload = new OSNotificationPayload();
		Dictionary<string, object> dictionary = jsonObject["payload"] as Dictionary<string, object>;
		if (dictionary.ContainsKey("notificationID"))
		{
			osnotificationPayload.notificationID = (dictionary["notificationID"] as string);
		}
		if (dictionary.ContainsKey("sound"))
		{
			osnotificationPayload.sound = (dictionary["sound"] as string);
		}
		if (dictionary.ContainsKey("title"))
		{
			osnotificationPayload.title = (dictionary["title"] as string);
		}
		if (dictionary.ContainsKey("body"))
		{
			osnotificationPayload.body = (dictionary["body"] as string);
		}
		if (dictionary.ContainsKey("subtitle"))
		{
			osnotificationPayload.subtitle = (dictionary["subtitle"] as string);
		}
		if (dictionary.ContainsKey("launchURL"))
		{
			osnotificationPayload.launchURL = (dictionary["launchURL"] as string);
		}
		if (dictionary.ContainsKey("additionalData"))
		{
			if (dictionary["additionalData"] is string)
			{
				osnotificationPayload.additionalData = (Json.Deserialize(dictionary["additionalData"] as string) as Dictionary<string, object>);
			}
			else
			{
				osnotificationPayload.additionalData = (dictionary["additionalData"] as Dictionary<string, object>);
			}
		}
		if (dictionary.ContainsKey("actionButtons"))
		{
			if (dictionary["actionButtons"] is string)
			{
				osnotificationPayload.actionButtons = (Json.Deserialize(dictionary["actionButtons"] as string) as Dictionary<string, object>);
			}
			else
			{
				osnotificationPayload.actionButtons = (dictionary["actionButtons"] as Dictionary<string, object>);
			}
		}
		if (dictionary.ContainsKey("contentAvailable"))
		{
			osnotificationPayload.contentAvailable = (bool)dictionary["contentAvailable"];
		}
		if (dictionary.ContainsKey("badge"))
		{
			osnotificationPayload.badge = Convert.ToInt32(dictionary["badge"]);
		}
		if (dictionary.ContainsKey("smallIcon"))
		{
			osnotificationPayload.smallIcon = (dictionary["smallIcon"] as string);
		}
		if (dictionary.ContainsKey("largeIcon"))
		{
			osnotificationPayload.largeIcon = (dictionary["largeIcon"] as string);
		}
		if (dictionary.ContainsKey("bigPicture"))
		{
			osnotificationPayload.bigPicture = (dictionary["bigPicture"] as string);
		}
		if (dictionary.ContainsKey("smallIconAccentColor"))
		{
			osnotificationPayload.smallIconAccentColor = (dictionary["smallIconAccentColor"] as string);
		}
		if (dictionary.ContainsKey("ledColor"))
		{
			osnotificationPayload.ledColor = (dictionary["ledColor"] as string);
		}
		if (dictionary.ContainsKey("lockScreenVisibility"))
		{
			osnotificationPayload.lockScreenVisibility = Convert.ToInt32(dictionary["lockScreenVisibility"]);
		}
		if (dictionary.ContainsKey("groupKey"))
		{
			osnotificationPayload.groupKey = (dictionary["groupKey"] as string);
		}
		if (dictionary.ContainsKey("groupMessage"))
		{
			osnotificationPayload.groupMessage = (dictionary["groupMessage"] as string);
		}
		if (dictionary.ContainsKey("fromProjectNumber"))
		{
			osnotificationPayload.fromProjectNumber = (dictionary["fromProjectNumber"] as string);
		}
		osnotification.payload = osnotificationPayload;
		if (jsonObject.ContainsKey("isAppInFocus"))
		{
			osnotification.isAppInFocus = (bool)jsonObject["isAppInFocus"];
		}
		if (jsonObject.ContainsKey("shown"))
		{
			osnotification.shown = (bool)jsonObject["shown"];
		}
		if (jsonObject.ContainsKey("silentNotification"))
		{
			osnotification.silentNotification = (bool)jsonObject["silentNotification"];
		}
		if (jsonObject.ContainsKey("androidNotificationId"))
		{
			osnotification.androidNotificationId = Convert.ToInt32(jsonObject["androidNotificationId"]);
		}
		if (jsonObject.ContainsKey("displayType"))
		{
			osnotification.displayType = (OSNotification.DisplayType)Convert.ToInt32(jsonObject["displayType"]);
		}
		return osnotification;
	}

	private void onPushNotificationReceived(string jsonString)
	{
		if (OneSignal.builder.notificationReceivedDelegate != null)
		{
			Dictionary<string, object> jsonObject = Json.Deserialize(jsonString) as Dictionary<string, object>;
			OneSignal.builder.notificationReceivedDelegate(this.DictionaryToNotification(jsonObject));
		}
	}

	private void onPushNotificationOpened(string jsonString)
	{
		if (OneSignal.builder.notificationOpenedDelegate != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(jsonString) as Dictionary<string, object>;
			OSNotificationAction osnotificationAction = new OSNotificationAction();
			if (dictionary.ContainsKey("action"))
			{
				Dictionary<string, object> dictionary2 = dictionary["action"] as Dictionary<string, object>;
				if (dictionary2.ContainsKey("actionID"))
				{
					osnotificationAction.actionID = (dictionary2["actionID"] as string);
				}
				if (dictionary2.ContainsKey("type"))
				{
					osnotificationAction.type = (OSNotificationAction.ActionType)Convert.ToInt32(dictionary2["type"]);
				}
			}
			OSNotificationOpenedResult osnotificationOpenedResult = new OSNotificationOpenedResult();
			osnotificationOpenedResult.notification = this.DictionaryToNotification((Dictionary<string, object>)dictionary["notification"]);
			osnotificationOpenedResult.action = osnotificationAction;
			OneSignal.builder.notificationOpenedDelegate(osnotificationOpenedResult);
		}
	}

	private void onIdsAvailable(string jsonString)
	{
		if (OneSignal.idsAvailableDelegate != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(jsonString) as Dictionary<string, object>;
			OneSignal.idsAvailableDelegate((string)dictionary["userId"], (string)dictionary["pushToken"]);
		}
	}

	private void onTagsReceived(string jsonString)
	{
		if (OneSignal.tagsReceivedDelegate != null)
		{
			OneSignal.tagsReceivedDelegate(Json.Deserialize(jsonString) as Dictionary<string, object>);
		}
	}

	private void onPostNotificationSuccess(string response)
	{
		if (OneSignal.postNotificationSuccessDelegate != null)
		{
			OneSignal.OnPostNotificationSuccess onPostNotificationSuccess = OneSignal.postNotificationSuccessDelegate;
			OneSignal.postNotificationFailureDelegate = null;
			OneSignal.postNotificationSuccessDelegate = null;
			onPostNotificationSuccess(Json.Deserialize(response) as Dictionary<string, object>);
		}
	}

	private void onSetEmailSuccess()
	{
		if (OneSignal.setEmailSuccessDelegate != null)
		{
			OneSignal.OnSetEmailSuccess onSetEmailSuccess = OneSignal.setEmailSuccessDelegate;
			OneSignal.setEmailSuccessDelegate = null;
			OneSignal.setEmailFailureDelegate = null;
			onSetEmailSuccess();
		}
	}

	private void onSetEmailFailure(string error)
	{
		if (OneSignal.setEmailFailureDelegate != null)
		{
			OneSignal.OnSetEmailFailure onSetEmailFailure = OneSignal.setEmailFailureDelegate;
			OneSignal.setEmailFailureDelegate = null;
			OneSignal.setEmailSuccessDelegate = null;
			onSetEmailFailure(Json.Deserialize(error) as Dictionary<string, object>);
		}
	}

	private void onLogoutEmailSuccess()
	{
		if (OneSignal.logoutEmailSuccessDelegate != null)
		{
			OneSignal.OnLogoutEmailSuccess onLogoutEmailSuccess = OneSignal.logoutEmailSuccessDelegate;
			OneSignal.logoutEmailSuccessDelegate = null;
			OneSignal.logoutEmailFailureDelegate = null;
			onLogoutEmailSuccess();
		}
	}

	private void onLogoutEmailFailure(string error)
	{
		if (OneSignal.logoutEmailFailureDelegate != null)
		{
			OneSignal.OnLogoutEmailFailure onLogoutEmailFailure = OneSignal.logoutEmailFailureDelegate;
			OneSignal.logoutEmailFailureDelegate = null;
			OneSignal.logoutEmailSuccessDelegate = null;
			onLogoutEmailFailure(Json.Deserialize(error) as Dictionary<string, object>);
		}
	}

	private void onPostNotificationFailed(string response)
	{
		if (OneSignal.postNotificationFailureDelegate != null)
		{
			OneSignal.OnPostNotificationFailure onPostNotificationFailure = OneSignal.postNotificationFailureDelegate;
			OneSignal.postNotificationFailureDelegate = null;
			OneSignal.postNotificationSuccessDelegate = null;
			onPostNotificationFailure(Json.Deserialize(response) as Dictionary<string, object>);
		}
	}

	private void onOSPermissionChanged(string stateChangesJSONString)
	{
		OSPermissionStateChanges stateChanges = OneSignal.oneSignalPlatform.parseOSPermissionStateChanges(stateChangesJSONString);
		OneSignal.internalPermissionObserver(stateChanges);
	}

	private void onOSSubscriptionChanged(string stateChangesJSONString)
	{
		OSSubscriptionStateChanges stateChanges = OneSignal.oneSignalPlatform.parseOSSubscriptionStateChanges(stateChangesJSONString);
		OneSignal.internalSubscriptionObserver(stateChanges);
	}

	private void onOSEmailSubscriptionChanged(string stateChangesJSONString)
	{
		OSEmailSubscriptionStateChanges stateChanges = OneSignal.oneSignalPlatform.parseOSEmailSubscriptionStateChanges(stateChangesJSONString);
		OneSignal.internalEmailSubscriptionObserver(stateChanges);
	}

	private void onPromptForPushNotificationsWithUserResponse(string accepted)
	{
		OneSignal.notificationUserResponseDelegate(Convert.ToBoolean(accepted));
	}

	public static OneSignal.IdsAvailableCallback idsAvailableDelegate;

	public static OneSignal.TagsReceived tagsReceivedDelegate;

	private static OneSignal.PromptForPushNotificationsUserResponse notificationUserResponseDelegate;

	private static OneSignal.PermissionObservable internalPermissionObserver;

	private static bool addedPermissionObserver;

	private static OneSignal.SubscriptionObservable internalSubscriptionObserver;

	private static bool addedSubscriptionObserver;

	private static OneSignal.EmailSubscriptionObservable internalEmailSubscriptionObserver;

	private static bool addedEmailSubscriptionObserver;

	public const string kOSSettingsAutoPrompt = "kOSSettingsAutoPrompt";

	public const string kOSSettingsInAppLaunchURL = "kOSSettingsInAppLaunchURL";

	internal static OneSignal.UnityBuilder builder;

	private static OneSignalPlatform oneSignalPlatform;

	private const string gameObjectName = "OneSignalRuntimeObject_KEEP";

	private static OneSignal.LOG_LEVEL logLevel = OneSignal.LOG_LEVEL.INFO;

	private static OneSignal.LOG_LEVEL visualLogLevel;

	internal static bool requiresUserConsent;

	internal static OneSignal.OnPostNotificationSuccess postNotificationSuccessDelegate;

	internal static OneSignal.OnPostNotificationFailure postNotificationFailureDelegate;

	internal static OneSignal.OnSetEmailSuccess setEmailSuccessDelegate;

	internal static OneSignal.OnSetEmailFailure setEmailFailureDelegate;

	internal static OneSignal.OnLogoutEmailSuccess logoutEmailSuccessDelegate;

	internal static OneSignal.OnLogoutEmailFailure logoutEmailFailureDelegate;

	private static OneSignal.OSInFocusDisplayOption _inFocusDisplayType = OneSignal.OSInFocusDisplayOption.InAppAlert;

	public delegate void NotificationReceived(OSNotification notification);

	public delegate void OnSetEmailSuccess();

	public delegate void OnSetEmailFailure(Dictionary<string, object> error);

	public delegate void OnLogoutEmailSuccess();

	public delegate void OnLogoutEmailFailure(Dictionary<string, object> error);

	public delegate void NotificationOpened(OSNotificationOpenedResult result);

	public delegate void IdsAvailableCallback(string playerID, string pushToken);

	public delegate void TagsReceived(Dictionary<string, object> tags);

	public delegate void PromptForPushNotificationsUserResponse(bool accepted);

	public delegate void OnPostNotificationSuccess(Dictionary<string, object> response);

	public delegate void OnPostNotificationFailure(Dictionary<string, object> response);

	public delegate void PermissionObservable(OSPermissionStateChanges stateChanges);

	public delegate void SubscriptionObservable(OSSubscriptionStateChanges stateChanges);

	public delegate void EmailSubscriptionObservable(OSEmailSubscriptionStateChanges stateChanges);

	public enum LOG_LEVEL
	{
		NONE,
		FATAL,
		ERROR,
		WARN,
		INFO,
		DEBUG,
		VERBOSE
	}

	public enum OSInFocusDisplayOption
	{
		None,
		InAppAlert,
		Notification
	}

	public class UnityBuilder
	{
		public OneSignal.UnityBuilder HandleNotificationReceived(OneSignal.NotificationReceived inNotificationReceivedDelegate)
		{
			this.notificationReceivedDelegate = inNotificationReceivedDelegate;
			return this;
		}

		public OneSignal.UnityBuilder HandleNotificationOpened(OneSignal.NotificationOpened inNotificationOpenedDelegate)
		{
			this.notificationOpenedDelegate = inNotificationOpenedDelegate;
			return this;
		}

		public OneSignal.UnityBuilder InFocusDisplaying(OneSignal.OSInFocusDisplayOption display)
		{
			OneSignal.inFocusDisplayType = display;
			return this;
		}

		public OneSignal.UnityBuilder Settings(Dictionary<string, bool> settings)
		{
			return this;
		}

		public void EndInit()
		{
			OneSignal.Init();
		}

		public OneSignal.UnityBuilder SetRequiresUserPrivacyConsent(bool required)
		{
			OneSignal.requiresUserConsent = true;
			return this;
		}

		public string appID;

		public string googleProjectNumber;

		public Dictionary<string, bool> iOSSettings;

		public OneSignal.NotificationReceived notificationReceivedDelegate;

		public OneSignal.NotificationOpened notificationOpenedDelegate;
	}
}
