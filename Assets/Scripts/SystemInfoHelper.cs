using System;
using UnityEngine;

public static class SystemInfoHelper
{
	public static string DeviceID
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfoHelper.deviceId))
			{
				SystemInfoHelper.deviceId = ActivityAndroid.uniqueGlobalDeviceIdentifier();
			}
			return SystemInfoHelper.deviceId;
		}
	}

	public static bool AdTrackingEnabled
	{
		get
		{
			return !ActivityAndroid.getIsAdvertisingInfoAvailable() || ActivityAndroid.getIsAdTrackingEnabled();
		}
	}

	public static string IFA
	{
		get
		{
			if (SystemInfoHelper.ifa == null)
			{
			}
			return SystemInfoHelper.ifa;
		}
	}

	public static string AID
	{
		get
		{
			if (SystemInfoHelper.aid == null)
			{
				if (!ActivityAndroid.getIsAdvertisingInfoAvailable())
				{
					return string.Empty;
				}
				SystemInfoHelper.aid = ActivityAndroid.getAdvertisingId();
				if (SystemInfoHelper.aid == null)
				{
					SystemInfoHelper.aid = string.Empty;
				}
			}
			return SystemInfoHelper.aid;
		}
	}

	public static string AdvertisingId
	{
		get
		{
			return SystemInfoHelper.AID;
		}
	}

	public static string IFV
	{
		get
		{
			if (SystemInfoHelper.ifv == null)
			{
			}
			return SystemInfoHelper.ifv;
		}
	}

	public static string Manufacturer
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfoHelper.manufacturer))
			{
				SystemInfoHelper.manufacturer = ActivityAndroid.getManufacturer();
			}
			return SystemInfoHelper.manufacturer;
		}
	}

	public static string BundleIdentifier
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfoHelper.bundleIdentifier))
			{
				SystemInfoHelper.bundleIdentifier = ActivityAndroid.getPackageName();
			}
			return SystemInfoHelper.bundleIdentifier;
		}
	}

	public static string BundleVersion
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfoHelper.bundleVersion))
			{
				SystemInfoHelper.bundleVersion = ActivityAndroid.getPackageVersionCode().ToString();
			}
			return SystemInfoHelper.bundleVersion;
		}
	}

	public static string BundleShortVersion
	{
		get
		{
			if (string.IsNullOrEmpty(SystemInfoHelper.bundleShortVersion))
			{
				SystemInfoHelper.bundleShortVersion = ActivityAndroid.getPackageVersionName();
			}
			return SystemInfoHelper.bundleShortVersion;
		}
	}

	public static int MajorVersion
	{
		get
		{
			return SystemInfoHelper.ParseMajorVersion(SystemInfoHelper.BundleShortVersion);
		}
	}

	public static int MinorVersion
	{
		get
		{
			return SystemInfoHelper.ParseMinorVersion(SystemInfoHelper.BundleShortVersion);
		}
	}

	public static string MajorAndMinorVersion
	{
		get
		{
			return SystemInfoHelper.MajorVersion + "." + SystemInfoHelper.MinorVersion;
		}
	}

	public static int PatchVersion
	{
		get
		{
			return SystemInfoHelper.ParsePatchVersion(SystemInfoHelper.BundleShortVersion);
		}
	}

	public static bool IsMajorVersionMatch(string version)
	{
		int num = SystemInfoHelper.ParseMajorVersion(version);
		return num == SystemInfoHelper.MajorVersion;
	}

	public static bool IsMinorVersionMatch(string version)
	{
		int num = SystemInfoHelper.ParseMinorVersion(version);
		return num == SystemInfoHelper.MinorVersion;
	}

	public static bool IsMajorAndMinorVersionMatch(string version)
	{
		string a = SystemInfoHelper.ParseMajorAndMinorVersion(version);
		return a == SystemInfoHelper.MajorAndMinorVersion;
	}

	public static bool IsPatchVersionMatch(string version)
	{
		int num = SystemInfoHelper.ParsePatchVersion(version);
		return num == SystemInfoHelper.PatchVersion;
	}

	public static bool IsTablet()
	{
		return DeviceUtilsAndroid.getScreenInches() > 6.5f;
	}

	public static bool IsKindleFire
	{
		get
		{
			return SystemInfoHelper.Manufacturer == "Amazon" && (SystemInfo.deviceModel == "Amazon Kindle Fire" || SystemInfo.deviceModel.StartsWith("Amazon KF"));
		}
	}

	public static string DeviceType
	{
		get
		{
			if (SystemInfoHelper.IsKindleFire)
			{
				return "FireOS";
			}
			return "Android";
		}
	}

	public static bool IsLowPerformanceDevice()
	{
		return false;
	}

	public static bool IsLowMemoryDevice()
	{
		return false;
	}

	public static string GetLocaleCountryCode()
	{
		return ActivityAndroid.getLocaleCountryCode();
	}

	public static string GetCurrentLocale()
	{
		return ActivityAndroid.getCurrentLocale();
	}

	private static int ParseMajorVersion(string version)
	{
		int num = version.IndexOf('.');
		if (num > 0)
		{
			version = version.Substring(0, num);
		}
		int result = 0;
		int.TryParse(version, out result);
		return result;
	}

	private static int ParseMinorVersion(string version)
	{
		int num = version.LastIndexOf('.');
		if (num > 0)
		{
			version = version.Substring(0, num);
		}
		int num2 = version.LastIndexOf('.');
		if (num2 > 0)
		{
			version = version.Substring(num2 + 1, version.Length - 1 - num2);
		}
		int result = 0;
		int.TryParse(version, out result);
		return result;
	}

	private static int ParsePatchVersion(string version)
	{
		int num = version.LastIndexOf('.');
		if (num > 0)
		{
			version = version.Substring(num + 1);
		}
		int result = 0;
		int.TryParse(version, out result);
		return result;
	}

	private static string ParseMajorAndMinorVersion(string version)
	{
		return SystemInfoHelper.ParseMajorVersion(version) + "." + SystemInfoHelper.ParseMinorVersion(version);
	}

	private static string deviceId;

	private static string ifa;

	private static string ifv;

	private static string aid;

	private static string manufacturer;

	private static string bundleIdentifier;

	private static string bundleVersion;

	private static string bundleShortVersion;
}
