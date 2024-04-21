using System;
using UnityEngine;

public static class JoinableEventHelper
{
	public static int SecondsLeft(DateTime evenStart, int eventDuration, int maxPlayTime, string startDateStr)
	{
		DateTime d = DateHelper.GetDateTimeFromString(startDateStr, "yyyy-MM-dd").AddSeconds((double)eventDuration);
		DateTime d2 = evenStart.AddSeconds((double)maxPlayTime);
		int num = (int)(d - d2).TotalSeconds;
		int b = maxPlayTime + num;
		int num2 = Mathf.Min(maxPlayTime, b);
		TimeSpan timeSpan = DateTime.UtcNow - evenStart;
		return (int)((double)num2 - timeSpan.TotalSeconds);
	}

	public static bool JoinWindowOpen(int joinWindowDuration, string startDateStr)
	{
		DateTime dateTimeFromString = DateHelper.GetDateTimeFromString(startDateStr, "yyyy-MM-dd");
		if (DateTime.UtcNow < dateTimeFromString)
		{
			return false;
		}
		DateTime utcNow = DateTime.UtcNow;
		return utcNow <= dateTimeFromString.AddSeconds((double)joinWindowDuration);
	}
}
