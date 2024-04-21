using System;
using System.Globalization;
using Localization;
using UnityEngine;

public static class L
{
	public static LanguageCode CurrentLanguageCode
	{
		get
		{
			return Internal.Instance.CurrentLanguageCode;
		}
	}

	public static string _(string key)
	{
		return L.Get(key);
	}

	public static string Get(string key)
	{
		return Internal.Instance.Get(key);
	}

	public static string FormatNumber(int number)
	{
		return Internal.Instance.FormatNumber(number);
	}

	public static string FormatNumber(long number)
	{
		return Internal.Instance.FormatNumber(number);
	}

	public static CultureInfo CurrentCultureInfo
	{
		get
		{
			return Internal.Instance.CurrentCultureInfo;
		}
	}

	public static char CurrentThousandSeperator
	{
		get
		{
			return Internal.Instance.CurrentThousandSeperator;
		}
	}

	public static LanguageCode LanguageCodeFromLanguageName(string languageName)
	{
		if (Enum.IsDefined(typeof(SystemLanguage), languageName))
		{
			SystemLanguage name = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
			return Internal.LanguageNameToCode(name);
		}
		return LanguageCode.N;
	}

	public static void UpdateLocalizationsWithAssetBundle(AssetBundle assetBundle)
	{
		Internal.Instance.UpdateSettings(assetBundle);
	}

	[Obsolete("Use FormatSecondsAsColumnSeparated(int, string, TimeFormatOptions) instead")]
	public static string FormatSecondsAsColumnSeparated(int totalSeconds, string timesUpString, bool isDaysFormattingAllowed)
	{
		TimeFormatOptions options = (!isDaysFormattingAllowed) ? TimeFormatOptions.DisallowDaysFormatting : TimeFormatOptions.None;
		return L.FormatSecondsAsColumnSeparated(totalSeconds, timesUpString, options);
	}

	public static string FormatSecondsAsColumnSeparated(int totalSeconds, string timesUpString, TimeFormatOptions options = TimeFormatOptions.None)
	{
		bool flag = (options & TimeFormatOptions.DisallowDaysFormatting) == TimeFormatOptions.None;
		bool flag2 = (options & TimeFormatOptions.HideHoursIfZero) == TimeFormatOptions.None;
		if (totalSeconds <= 0)
		{
			return timesUpString;
		}
		int num = totalSeconds / 3600;
		int num2 = totalSeconds % 3600 / 60;
		int num3 = totalSeconds % 3600 % 60;
		if (flag && num > 24)
		{
			if (num < 48)
			{
				return L.Get("1 day");
			}
			return num / 24 + L.Get(" days");
		}
		else
		{
			if (flag2 || num != 0)
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2}", num, num2, num3);
			}
			return string.Format("{0:D2}:{1:D2}", num2, num3);
		}
	}
}
