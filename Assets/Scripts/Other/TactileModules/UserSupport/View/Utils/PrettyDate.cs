using System;

namespace TactileModules.UserSupport.View.Utils
{
	public class PrettyDate
	{
		public static string GetPrettyDate(DateTime d)
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(d);
			int num = (int)timeSpan.TotalDays;
			int num2 = (int)timeSpan.TotalSeconds;
			if (num < 0 || num >= 31)
			{
				return null;
			}
			if (num == 0)
			{
				if (num2 < 60)
				{
					return "just now";
				}
				if (num2 < 120)
				{
					return "1 minute ago";
				}
				if (num2 < 3600)
				{
					return string.Format("{0} minutes ago", Math.Floor((double)num2 / 60.0));
				}
				if (num2 < 7200)
				{
					return "1 hour ago";
				}
				if (num2 < 86400)
				{
					return string.Format("{0} hours ago", Math.Floor((double)num2 / 3600.0));
				}
			}
			if (num == 1)
			{
				return "yesterday";
			}
			if (num < 7)
			{
				return string.Format("{0} days ago", num);
			}
			if (num < 31)
			{
				return string.Format("{0} weeks ago", Math.Ceiling((double)num / 7.0));
			}
			return null;
		}
	}
}
