using System;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.FeatureManager
{
	public class FeatureTypeUrlFileCachingFactory : IFeatureTypeUrlFileCachingFactory
	{
		public IFeatureTypeUrlFileCaching Create(IFeatureUrlFileHandler featureUrlFileHandler, IUrlCacher urlCacher)
		{
			return new FeatureTypeUrlFileCaching(featureUrlFileHandler, urlCacher);
		}
	}
}
