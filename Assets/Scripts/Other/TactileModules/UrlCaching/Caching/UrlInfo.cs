using System;
using System.IO;

namespace TactileModules.UrlCaching.Caching
{
	public static class UrlInfo
	{
		public static string GetFileExtension(string url)
		{
			return Path.GetExtension(url);
		}

		public static bool IsValidUrl(string url)
		{
			return url.Contains("https");
		}
	}
}
