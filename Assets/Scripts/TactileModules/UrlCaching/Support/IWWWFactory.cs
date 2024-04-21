using System;

namespace TactileModules.UrlCaching.Support
{
	public interface IWWWFactory
	{
		IWWW CreateWWW(string url);
	}
}
