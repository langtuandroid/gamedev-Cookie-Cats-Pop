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
		//AndroidNotifications._plugin = AndroidPluginManager.GetPlugin("dk.tactile.notifications.AndroidNotificationsPlugin");
		//AndroidNotifications.CreateNotificationChannel();
	}

	public static void CreateNotificationChannel()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		//AndroidNotifications._plugin.Call("createNotificationChannel", new object[0]);
	}

	public static int ScheduleNotification(long secondsFromNow, string subtitle, string tickerText)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1;
		}
        //return AndroidNotifications._plugin.Call<int>("scheduleNotification", new object[]
        //{
        //	secondsFromNow,
        //	subtitle,
        //	tickerText
        //});
        return -1;
    }

	public static void CancelNotification(int Id)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		//AndroidNotifications._plugin.Call("cancelNotification", new object[]
		//{
		//	Id
		//});
	}

	public static void UpdateNotification(int id, long secondsFromNow, string subtitle, string tickerText)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		//AndroidNotifications._plugin.Call("updateNotification", new object[]
		//{
		//	id,
		//	secondsFromNow,
		//	subtitle,
		//	tickerText
		//});
	}

	private static AndroidJavaObject _plugin;
}
