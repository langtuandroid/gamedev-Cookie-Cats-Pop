using System;
using UnityEngine;

public static class ActivityAndroid
{
	static ActivityAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		//ActivityAndroid._plugin = AndroidPluginManager.GetPlugin("dk.tactile.activity.ActivityPlugin");
	}

	public static string getManufacturer()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		return "BigBoss";
	}

	public static string getPackageName()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		return "com.bigboss.cookiecat";
	}

	public static int getPackageVersionCode()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1;
		}
		return 1;
	}

	public static string getPackageVersionName()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		return "1.0";
	}

	public static string uniqueGlobalDeviceIdentifier()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
		return string.Empty;
	}

	public static bool isPackageInstalled(string packageName)
	{
		return Application.platform == RuntimePlatform.Android && ActivityAndroid._plugin.Call<bool>("isPackageInstalled", new object[]
		{
			packageName
		});
	}

	public static void launchOtherApp(string packageName)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		ActivityAndroid._plugin.Call("launchOtherApp", new object[]
		{
			packageName
		});
	}

	public static void launchOtherApp(string packageName, string url)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		ActivityAndroid._plugin.Call("launchOtherApp", new object[]
		{
			packageName,
			url
		});
	}

	public static bool getIsAdvertisingInfoAvailable()
	{
		return true;
	}

	public static string getAdvertisingId()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
        //return ActivityAndroid._plugin.Call<string>("getAdvertisingId", new object[0]);
        return string.Empty;
	}

	public static bool getIsAdTrackingEnabled()
	{
		return true;
	}

	public static void requestPermissions(string[] permissions, int requestCode)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		//ActivityAndroid._plugin.Call("requestPermissions", new object[]
		//{
		//	permissions,
		//	requestCode
		//});
	}

	public static bool checkSelfPermission(string permission)
	{
        //return Application.platform == RuntimePlatform.Android && ActivityAndroid._plugin.Call<bool>("checkSelfPermission", new object[]
        //{
        //	permission
        //});
        return true;
    }

	public static bool shouldShowRequestPermissionRationale(string permission)
	{
        //return Application.platform == RuntimePlatform.Android && ActivityAndroid._plugin.Call<bool>("shouldShowRequestPermissionRationale", new object[]
        //{
        //	permission
        //});
        return true;
    }

	public static string getLocaleCountryCode()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
        //return ActivityAndroid._plugin.Call<string>("getLocaleCountryCode", new object[0]);
        return "en";
    }

	public static string getCurrentLocale()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return string.Empty;
		}
        return "en";
    }

	public static bool externalStorageAvailable()
	{
		return Application.platform == RuntimePlatform.Android ;
	}

	public static long getAvailableInternalStorageSize()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1L;
		}
		return -1L;
	}

	public static long getTotalInternalStorageSize()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1L;
		}
		return -1L;
	}

	public static long getAvailableExternalStorageSize()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1L;
		}
		return -1L;
	}

	public static long getTotalExternalStorageSize()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return -1L;
		}
		return -1L;
	}

	private static AndroidJavaObject _plugin;
}
