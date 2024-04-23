using System;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureTypeUrlFileCachingFactory
	{
		IFeatureTypeUrlFileCaching Create(IFeatureUrlFileHandler featureUrlFileHandler, IUrlCacher urlCacher);
	}
}
