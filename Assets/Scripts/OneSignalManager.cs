using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public class OneSignalManager : SingleInstance<OneSignalManager>
{
	public OneSignalManager(string appId, string gcmSenderId, CloudClientBase cloudClient)
	{
		this.cloudClient = cloudClient;
		OneSignal.idsAvailableDelegate = new OneSignal.IdsAvailableCallback(this.HandleIdsAvailable);
		OneSignal.tagsReceivedDelegate = new OneSignal.TagsReceived(this.HandleTagsReceived);
		OneSignal.StartInit(appId, gcmSenderId).InFocusDisplaying(OneSignal.OSInFocusDisplayOption.None).HandleNotificationReceived(new OneSignal.NotificationReceived(this.HandleNotificationRecieved)).HandleNotificationOpened(new OneSignal.NotificationOpened(this.HandleNotificationOpened)).Settings(new Dictionary<string, bool>
		{
			{
				"kOSSettingsAutoPrompt",
				false
			}
		}).SetRequiresUserPrivacyConsent(true).EndInit();
		OneSignal.UserDidProvideConsent(true);
		if (this.InitialRemoteRegistrationCompleted)
		{
			this.RegisterForPush();
		}
		OneSignal.IdsAvailable();
		OneSignal.ClearOneSignalNotifications();
		ActivityManager.onResumeEvent += this.ApplicationWillEnterForeground;
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<OSNotification> NotificationRecieved;

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<OSNotificationOpenedResult> NotificationOpened;

	public bool IsRegisteredForNotifications { get; private set; }

	private void ApplicationWillEnterForeground()
	{
		this.registeredForRemoteNotificationsThisSession = false;
		OneSignal.ClearOneSignalNotifications();
		if (this.InitialRemoteRegistrationCompleted)
		{
			this.RegisterForPush();
		}
	}

	public void RegisterForPush()
	{
		if (this.registeredForRemoteNotificationsThisSession)
		{
			return;
		}
		this.registeredForRemoteNotificationsThisSession = true;
		this.InitialRemoteRegistrationCompleted = true;
		OneSignal.IdsAvailable();
		OneSignal.RegisterForPushNotifications();
	}

	public void SetSubscription(bool enable)
	{
		OneSignal.SetSubscription(enable);
	}

	private void HandleIdsAvailable(string playerID, string pushToken)
	{
		this.OneSignalPlayerId = playerID;
		this.IsRegisteredForNotifications = !string.IsNullOrEmpty(pushToken);
		if (!string.IsNullOrEmpty(pushToken))
		{
			TactileAnalytics.Instance.LogEvent(new OneSignalManager.RegisteredForPushEvent(pushToken), -1.0, null);
		}
	}

	private string DictToString(Dictionary<string, object> dict)
	{
		if (dict == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		bool flag = true;
		foreach (KeyValuePair<string, object> keyValuePair in dict)
		{
			if (!flag)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(keyValuePair.Key.ToString() + "=" + keyValuePair.Value.ToString());
			flag = false;
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private void HandleTagsReceived(Dictionary<string, object> tags)
	{
	}

	private void HandleNotificationRecieved(OSNotification notification)
	{
		if (this.NotificationRecieved != null)
		{
			this.NotificationRecieved(notification);
		}
	}

	private void HandleNotificationOpened(OSNotificationOpenedResult result)
	{
		if (this.NotificationOpened != null)
		{
			this.NotificationOpened(result);
		}
	}

	private bool InitialRemoteRegistrationCompleted
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public string OneSignalPlayerId
	{
		get
		{
			return TactilePlayerPrefs.GetString("OneSignalManagerPlayerId", string.Empty);
		}
		private set
		{
			TactilePlayerPrefs.SetString("OneSignalManagerPlayerId", value);
		}
	}

	private bool registeredForRemoteNotificationsThisSession;

	[Obsolete("This action is no longer in use. Use one of the two other actions instead.", true)]
	public Action<string, Dictionary<string, object>, bool> NotificationReceived;

	private CloudClientBase cloudClient;

	[TactileAnalytics.EventAttribute("registeredForPush", true)]
	private class RegisteredForPushEvent
	{
		public RegisteredForPushEvent(string pushToken)
		{
			this.AndroidRegistrationId = pushToken;
		}

		private TactileAnalytics.OptionalParam<string> AndroidRegistrationId { get; set; }

		private TactileAnalytics.OptionalParam<string> PushNotificationToken { get; set; }
	}
}
