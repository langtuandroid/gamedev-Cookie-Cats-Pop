using System;

namespace TactileModules.UrlCaching.Support
{
	public class WWWFactory : IWWWFactory
	{
		public IWWW CreateWWW(string url)
		{
			return new WWW(url);
		}
	}
}
