using System;

namespace TactileModules.UrlCaching.Caching
{
	public interface IUrlCacherFactory
	{
		IUrlCacher Create();

		IUrlCacher Create(string domain);
	}
}
