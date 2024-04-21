using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ClientDeviceEventUtils
{
	public static string GetDeviceName()
	{
		string deviceModel = SystemInfo.deviceModel;
		switch (deviceModel)
		{
		case "iPhone1,1":
			return "iPhone 1G";
		case "iPhone1,2":
			return "iPhone 3G";
		case "iPhone2,1":
			return "iPhone 3GS";
		case "iPhone3,1":
			return "iPhone 4";
		case "iPhone3,2":
			return "iPhone 4";
		case "iPhone3,3":
			return "iPhone 4";
		case "iPhone4,1":
			return "iPhone 4S";
		case "iPhone5,1":
			return "iPhone 5";
		case "iPhone5,2":
			return "iPhone 5";
		case "iPhone5,3":
			return "iPhone 5C";
		case "iPhone5,4":
			return "iPhone 5C";
		case "iPhone6,1":
			return "iPhone 5S";
		case "iPhone6,2":
			return "iPhone 5S";
		case "iPhone7,2":
			return "iPhone 6";
		case "iPhone7,1":
			return "iPhone 6 Plus";
		case "iPhone8,1":
			return "iPhone 6s";
		case "iPhone8,2":
			return "iPhone 6s Plus";
		case "iPhone8,4":
			return "iPhone SE";
		case "iPhone9,1":
			return "iPhone 7";
		case "iPhone9,3":
			return "iPhone 7";
		case "iPhone9,2":
			return "iPhone 7 Plus";
		case "iPhone9,4":
			return "iPhone 7 Plus";
		case "iPod1,1":
			return "iPod Touch 1";
		case "iPod2,1":
			return "iPod Touch 2";
		case "iPod3,1":
			return "iPod Touch 3";
		case "iPod4,1":
			return "iPod Touch 4";
		case "iPod5,1":
			return "iPod Touch 5";
		case "iPod7,1":
			return "iPod Touch 6";
		case "iPad1,1":
			return "iPad 1";
		case "iPad2,1":
			return "iPad 2";
		case "iPad2,2":
			return "iPad 2";
		case "iPad2,3":
			return "iPad 2";
		case "iPad2,4":
			return "iPad 2";
		case "iPad3,1":
			return "iPad 3";
		case "iPad3,2":
			return "iPad 3";
		case "iPad3,3":
			return "iPad 3";
		case "iPad3,4":
			return "iPad 4";
		case "iPad3,5":
			return "iPad 4";
		case "iPad3,6":
			return "iPad 4";
		case "iPad4,1":
			return "iPad Air";
		case "iPad4,2":
			return "iPad Air";
		case "iPad4,3":
			return "iPad Air";
		case "iPad5,3":
			return "iPad Air 2";
		case "iPad5,4":
			return "iPad Air 2";
		case "iPad6,3":
			return "iPad Pro (9.7 inch)";
		case "iPad6,4":
			return "iPad Pro (9.7 inch)";
		case "iPad6,7":
			return "iPad Pro (12.9 inch)";
		case "iPad6,8":
			return "iPad Pro (12.9 inch)";
		case "iPad6,11":
			return "iPad 5";
		case "iPad6,12":
			return "iPad 5";
		case "iPad2,5":
			return "iPad Mini 1";
		case "iPad2,6":
			return "iPad Mini 1";
		case "iPad2,7":
			return "iPad Mini 1";
		case "iPad4,4":
			return "iPad Mini 2";
		case "iPad4,5":
			return "iPad Mini 2";
		case "iPad4,6":
			return "iPad Mini 2";
		case "iPad4,7":
			return "iPad Mini 3";
		case "iPad4,8":
			return "iPad Mini 3";
		case "iPad4,9":
			return "iPad Mini 3";
		case "iPad5,1":
			return "iPad Mini 4";
		case "iPad5,2":
			return "iPad Mini 4";
		case "Amazon KFGIWI":
			return "Fire HD 8 (2016)";
		case "Amazon KFTBWI":
			return "Fire HD 10 (2015)";
		case "Amazon KFMEWI":
			return "Fire HD 8 (2015)";
		case "Amazon KFFOWI":
			return "Fire (2015)";
		case "Amazon KFSAWA":
			return "Fire HDX 8.9 (2014)";
		case "Amazon KFSAWI":
			return "Fire HDX 8.9 (2014)";
		case "Amazon KFASWI":
			return "Fire HD 7 (2014)";
		case "Amazon KFARWI":
			return "Fire HD 6 (2014)";
		case "Amazon KFAPWA":
			return "Kindle Fire HDX 8.9 (2013)";
		case "Amazon KFAPWI":
			return "Kindle Fire HDX 8.9 (2013)";
		case "Amazon KFTHWA":
			return "Kindle Fire HDX 7 (2013)";
		case "Amazon KFTHWI":
			return "Kindle Fire HDX 7 (2013)";
		case "Amazon KFSOWI":
			return "Kindle Fire HD 7 (2013)";
		case "Amazon KFJWA":
			return "Kindle Fire HD 8.9 (2012)";
		case "Amazon KFJWI":
			return "Kindle Fire HD 8.9 (2012)";
		case "Amazon KFTT":
			return "Kindle Fire HD 7 (2012)";
		case "Amazon KFOT":
			return "Kindle Fire (2012)";
		case "Amazon Kindle Fire":
			return "Kindle Fire (2011)";
		}
		return deviceModel;
	}

	public static string GetDeviceType()
	{
		switch (SystemInfo.deviceType)
		{
		case DeviceType.Handheld:
			return (!SystemInfoHelper.IsTablet()) ? "MOBILE_PHONE" : "TABLET";
		case DeviceType.Console:
			return "CONSOLE";
		case DeviceType.Desktop:
			return "PC";
		default:
			return "UNKNOWN";
		}
	}

	public static string GetOperatingSystem()
	{
		string text = SystemInfo.operatingSystem.ToUpper();
		if (text.Contains("WINDOWS"))
		{
			return "WINDOWS";
		}
		if (text.Contains("OSX") || text.Contains("MAC"))
		{
			return "OSX";
		}
		if (text.Contains("IOS") || text.Contains("IPHONE") || text.Contains("IPAD"))
		{
			return "IOS";
		}
		if (text.Contains("LINUX"))
		{
			return "LINUX";
		}
		if (text.Contains("ANDROID"))
		{
			if (SystemInfo.deviceModel.ToUpper().Contains("AMAZON"))
			{
				return "FIREOS";
			}
			return "ANDROID";
		}
		else
		{
			if (text.Contains("BLACKBERRY"))
			{
				return "BLACKBERRY";
			}
			return "UNKNOWN";
		}
	}

	public static string GetOperatingSystemVersion()
	{
		string pattern = ".*OS ([0-9A-Za-z\\.]+) \\/.*";
		Regex regex = new Regex(pattern);
		Match match = regex.Match(SystemInfo.operatingSystem);
		if (match.Success)
		{
			return match.Groups[1].Value;
		}
		return SystemInfo.operatingSystem;
	}

	public static string GetTimezoneOffset()
	{
		string result;
		try
		{
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			DateTime now = DateTime.Now;
			TimeSpan utcOffset = currentTimeZone.GetUtcOffset(now);
			result = string.Format("{0}{1:D2}", (utcOffset.Hours < 0) ? string.Empty : "+", utcOffset.Hours);
		}
		catch (Exception)
		{
			result = null;
		}
		return result;
	}

	public static string GetDeviceLanguageCode()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Afrikaans:
			return "af";
		case SystemLanguage.Arabic:
			return "ar";
		case SystemLanguage.Basque:
			return "eu";
		case SystemLanguage.Belarusian:
			return "be";
		case SystemLanguage.Bulgarian:
			return "bg";
		case SystemLanguage.Catalan:
			return "ca";
		case SystemLanguage.Chinese:
			return "zh";
		case SystemLanguage.Czech:
			return "cs";
		case SystemLanguage.Danish:
			return "da";
		case SystemLanguage.Dutch:
			return "nl";
		case SystemLanguage.English:
			return "en";
		case SystemLanguage.Estonian:
			return "et";
		case SystemLanguage.Faroese:
			return "fo";
		case SystemLanguage.Finnish:
			return "fi";
		case SystemLanguage.French:
			return "fr";
		case SystemLanguage.German:
			return "de";
		case SystemLanguage.Greek:
			return "el";
		case SystemLanguage.Hebrew:
			return "he";
		case SystemLanguage.Hungarian:
			return "hu";
		case SystemLanguage.Icelandic:
			return "is";
		case SystemLanguage.Indonesian:
			return "id";
		case SystemLanguage.Italian:
			return "it";
		case SystemLanguage.Japanese:
			return "ja";
		case SystemLanguage.Korean:
			return "ko";
		case SystemLanguage.Latvian:
			return "lv";
		case SystemLanguage.Lithuanian:
			return "lt";
		case SystemLanguage.Norwegian:
			return "nn";
		case SystemLanguage.Polish:
			return "pl";
		case SystemLanguage.Portuguese:
			return "pt";
		case SystemLanguage.Romanian:
			return "ro";
		case SystemLanguage.Russian:
			return "ru";
		case SystemLanguage.SerboCroatian:
			return "sr";
		case SystemLanguage.Slovak:
			return "sk";
		case SystemLanguage.Slovenian:
			return "sl";
		case SystemLanguage.Spanish:
			return "es";
		case SystemLanguage.Swedish:
			return "sv";
		case SystemLanguage.Thai:
			return "th";
		case SystemLanguage.Turkish:
			return "tr";
		case SystemLanguage.Ukrainian:
			return "uk";
		case SystemLanguage.Vietnamese:
			return "vi";
		case SystemLanguage.ChineseSimplified:
			return "zh";
		default:
			return "en";
		}
	}
}
