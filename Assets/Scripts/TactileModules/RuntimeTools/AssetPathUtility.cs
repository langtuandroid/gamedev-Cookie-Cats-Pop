using System;

namespace TactileModules.RuntimeTools
{
	public static class AssetPathUtility
	{
		public static string AssetPathToResourcePath(string assetPath)
		{
			int num = assetPath.IndexOf("Resources/");
			string text = assetPath.Substring(num + 10);
			num = text.LastIndexOf(".");
			if (num < 0)
			{
				return text;
			}
			return text.Substring(0, num);
		}

		public static string AssetPathToUniqueIdentifier(string assetPath)
		{
			string text = assetPath.Substring(0, assetPath.LastIndexOf('.'));
			return text.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
		}
	}
}
