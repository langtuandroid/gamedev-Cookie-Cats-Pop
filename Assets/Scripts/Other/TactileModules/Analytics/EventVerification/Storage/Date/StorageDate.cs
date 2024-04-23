using System;

namespace TactileModules.Analytics.EventVerification.Storage.Date
{
	public class StorageDate : IStorageDate
	{
		public static int DaysBetween(string dateA, string dateB)
		{
			DateTime d = DateTime.ParseExact(dateA, "yyyy-MM-dd", null);
			DateTime d2 = DateTime.ParseExact(dateB, "yyyy-MM-dd", null);
			return (d - d2).Days;
		}

		public static string FromUnixTimeStamp(long timestamp)
		{
			return DateHelper.GetDateTimeFromUnixTimestamp(timestamp).ToString("yyyy-MM-dd");
		}

		public string Today()
		{
			return DateTime.UtcNow.ToString("yyyy-MM-dd");
		}

		public const string DATE_FORMAT = "yyyy-MM-dd";
	}
}
