using System;

public static class DateTimeExtensions
{
	public static double ToUtcEpoch(this DateTime value)
	{
		return (value - DateTimeExtensions.epoch).TotalSeconds;
	}

	public static DateTime ToDateTimeFromEpoch(this long intDate)
	{
		long value = intDate * 10000000L;
		return DateTimeExtensions.epoch.AddTicks(value);
	}

	public static DateTime ToDateTimeFromEpochInt(this int intDate)
	{
		long value = (long)intDate * 10000000L;
		return DateTimeExtensions.epoch.AddTicks(value);
	}

	private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}
