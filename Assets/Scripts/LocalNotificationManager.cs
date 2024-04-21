using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalNotificationManager
{
	public static void Schedule(string noteId, LocalNotificationManager.Notification note)
	{
		List<LocalNotificationManager.Notification> persistedNotifications = LocalNotificationManager.PersistedNotifications;
		bool flag = false;
		int num = Mathf.Max(0, Mathf.CeilToInt((float)(note.WhenEpochSeconds - DateTime.UtcNow.ToUtcEpoch())));
		if (note.UserInfo == null)
		{
			note.UserInfo = new Dictionary<string, string>();
		}
		note.UserInfo["noteId"] = noteId;
		for (int i = persistedNotifications.Count - 1; i >= 0; i--)
		{
			LocalNotificationManager.Notification notification = persistedNotifications[i];
			if (notification.UserInfo.ContainsKey("noteId") && notification.UserInfo["noteId"] == noteId && notification.UserInfo.ContainsKey("androidNoteId"))
			{
				note.UserInfo.Add("androidNoteId", notification.UserInfo["androidNoteId"]);
				persistedNotifications[i] = note;
				flag = true;
				if (LocalNotificationManager.Enabled)
				{
					AndroidNotifications.UpdateNotification(int.Parse(note.UserInfo["androidNoteId"]), (long)num, note.AlertBody, note.AlertAction);
				}
				break;
			}
		}
		if (!flag)
		{
			if (LocalNotificationManager.Enabled)
			{
				int num2 = AndroidNotifications.ScheduleNotification((long)num, note.AlertBody, note.AlertAction);
				note.UserInfo.Add("androidNoteId", num2.ToString());
			}
			persistedNotifications.Add(note);
		}
		LocalNotificationManager.PersistedNotifications = persistedNotifications;
	}

	public static void Cancel(string noteId)
	{
		List<LocalNotificationManager.Notification> persistedNotifications = LocalNotificationManager.PersistedNotifications;
		for (int i = persistedNotifications.Count - 1; i >= 0; i--)
		{
			LocalNotificationManager.Notification notification = persistedNotifications[i];
			if (notification.UserInfo.ContainsKey("noteId") && notification.UserInfo["noteId"] == noteId)
			{
				if (notification.UserInfo.ContainsKey("androidNoteId") && LocalNotificationManager.Enabled)
				{
					AndroidNotifications.CancelNotification(int.Parse(notification.UserInfo["androidNoteId"]));
				}
				persistedNotifications.RemoveAt(i);
			}
		}
		LocalNotificationManager.PersistedNotifications = persistedNotifications;
	}

	public static bool IsScheduled(string noteId)
	{
		List<LocalNotificationManager.Notification> persistedNotifications = LocalNotificationManager.PersistedNotifications;
		for (int i = persistedNotifications.Count - 1; i >= 0; i--)
		{
			LocalNotificationManager.Notification notification = persistedNotifications[i];
			if (notification.UserInfo.ContainsKey("noteId") && notification.UserInfo["noteId"] == noteId)
			{
				return true;
			}
		}
		return false;
	}

	private static void DisableNotifications()
	{
		List<LocalNotificationManager.Notification> persistedNotifications = LocalNotificationManager.PersistedNotifications;
		foreach (LocalNotificationManager.Notification notification in persistedNotifications)
		{
			if (notification.UserInfo.ContainsKey("androidNoteId"))
			{
				AndroidNotifications.CancelNotification(int.Parse(notification.UserInfo["androidNoteId"]));
				notification.UserInfo.Remove("androidNoteId");
			}
		}
	}

	private static void EnableNotifications()
	{
		List<LocalNotificationManager.Notification> persistedNotifications = LocalNotificationManager.PersistedNotifications;
		foreach (LocalNotificationManager.Notification notification in persistedNotifications)
		{
			int num = Mathf.Max(0, Mathf.CeilToInt((float)(notification.WhenEpochSeconds - DateTime.UtcNow.ToUtcEpoch())));
			int num2 = AndroidNotifications.ScheduleNotification((long)num, notification.AlertBody, notification.AlertAction);
			if (notification.UserInfo.ContainsKey("androidNoteId"))
			{
				notification.UserInfo["androidNoteId"] = num2.ToString();
			}
			else
			{
				notification.UserInfo.Add("androidNoteId", num2.ToString());
			}
		}
	}

	private static void RemoveExpired(List<LocalNotificationManager.Notification> notifications)
	{
		for (int i = notifications.Count - 1; i >= 0; i--)
		{
			if (notifications[i].WhenEpochSeconds < DateTime.UtcNow.ToUtcEpoch())
			{
				notifications.RemoveAt(i);
			}
		}
	}

	private static void RemoveExpired(ArrayList notifications)
	{
		for (int i = notifications.Count - 1; i >= 0; i--)
		{
			Hashtable hashtable = (Hashtable)notifications[i];
			if (hashtable.ContainsKey("epochRelavtiveTimeInSeconds"))
			{
				if ((double)hashtable["epochRelavtiveTimeInSeconds"] < DateTime.UtcNow.ToUtcEpoch())
				{
					notifications.RemoveAt(i);
				}
			}
			else
			{
				notifications.RemoveAt(i);
			}
		}
	}

	private static List<LocalNotificationManager.Notification> PersistedNotifications
	{
		get
		{
			string securedString = TactilePlayerPrefs.GetSecuredString(LocalNotificationManager.PREFS_NOTES, string.Empty);
			if (securedString.Length > 0)
			{
				List<LocalNotificationManager.Notification> list = JsonSerializer.ArrayListToGenericList<LocalNotificationManager.Notification>(securedString.arrayListFromJson());
				LocalNotificationManager.RemoveExpired(list);
				return list;
			}
			return new List<LocalNotificationManager.Notification>();
		}
		set
		{
			if (value != null)
			{
				LocalNotificationManager.RemoveExpired(value);
				TactilePlayerPrefs.SetSecuredString(LocalNotificationManager.PREFS_NOTES, JsonSerializer.GenericListToArrayList<LocalNotificationManager.Notification>(value).toJson());
			}
			else
			{
				TactilePlayerPrefs.SetSecuredString(LocalNotificationManager.PREFS_NOTES, string.Empty);
			}
		}
	}

	public static bool Enabled
	{
		get
		{
			return TactilePlayerPrefs.GetBool(LocalNotificationManager.PREFS_ENABLED, true);
		}
		set
		{
			if (value != LocalNotificationManager.Enabled)
			{
				if (value)
				{
					LocalNotificationManager.EnableNotifications();
				}
				else
				{
					LocalNotificationManager.DisableNotifications();
				}
				TactilePlayerPrefs.SetBool(LocalNotificationManager.PREFS_ENABLED, value);
			}
		}
	}

	private static string PREFS_NOTES = "LocalNotificationManagerNotes";

	private static string PREFS_ENABLED = "LocalNotificationManagerEnabled";

	public class Notification
	{
		[JsonSerializable("epochRelavtiveTimeInSeconds", null)]
		public double WhenEpochSeconds { get; set; }

		[JsonSerializable("alertBody", null)]
		public string AlertBody { get; set; }

		[JsonSerializable("alertAction", null)]
		public string AlertAction { get; set; }

		[JsonSerializable("badgeCount", null)]
		public int BadgeCount { get; set; }

		[JsonSerializable("launchImage", null)]
		public string LaunchImage { get; set; }

		[JsonSerializable("sound", null)]
		public string Sound { get; set; }

		[JsonSerializable("userInfo", typeof(string))]
		public Dictionary<string, string> UserInfo { get; set; }

		[JsonSerializable("wallClockTime", null)]
		public bool WallClockTime { get; set; }
	}
}
