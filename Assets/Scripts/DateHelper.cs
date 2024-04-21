using System;
using System.Globalization;

public static class DateHelper
{
	public static int GetDayID()
	{
		return (DateTime.Now - DateHelper.referenceTime).Days;
	}

	public static DateTime GetLocalTimeFromDayID(int dayID)
	{
		return DateHelper.referenceTime.AddDays((double)dayID);
	}

	public static int GetDayIDFromDate(DateTime date)
	{
		return (date - DateHelper.referenceTime).Days;
	}

	public static int GetHourID()
	{
		return (int)(DateTime.Now - DateHelper.referenceTime).TotalHours;
	}

	public static int DifferenceInDays(DateTime d1, DateTime d2, bool absolute = true)
	{
		TimeSpan timeSpan = d2 - d1;
		return (!absolute) ? timeSpan.Days : Math.Abs(timeSpan.Days);
	}

	public static DateTime DateTimeFromDayId(int dayId)
	{
		return DateHelper.referenceTime.AddDays((double)dayId);
	}

	public static DateTime DefaultTime
	{
		get
		{
			return new DateTime(2000, 1, 1);
		}
	}

	public static DateTime GetDateTimeFromString(string val, string dateFormat = "yyyy-MM-dd")
	{
		DateTime result;
		DateTime.TryParseExact(val, dateFormat, null, DateTimeStyles.None, out result);
		return result;
	}

	public static DateTime GetDateTimeFromUnixTimestamp(long timestamp)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return dateTime.AddSeconds((double)timestamp);
	}

	public static double GetUnixTimestampFromDateTime(DateTime date)
	{
		DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		return Math.Floor((date.ToUniversalTime() - d).TotalSeconds);
	}

	private static DateTime referenceTime = new DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Local);
}
