using System;
using UnityEngine;

public class AndroidNotifications
{
	static AndroidNotifications()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}

	}

	public static void CreateNotificationChannel()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
	}

	public static int ScheduleNotification(long secondsFromNow, string subtitle, string tickerText)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1;
		}
        return -1;
    }

	public static void CancelNotification(int Id)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
	}

	public static void UpdateNotification(int id, long secondsFromNow, string subtitle, string tickerText)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
	}

	private static AndroidJavaObject _plugin;
}
