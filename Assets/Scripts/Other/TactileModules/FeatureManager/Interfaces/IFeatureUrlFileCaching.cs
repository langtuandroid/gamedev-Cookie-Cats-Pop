using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureUrlFileCaching
	{
		event Action OnUrlFileCached;

		void CacheUrlFilesForFeatures(List<FeatureData> featureInstances, List<IFeatureTypeHandler> featureHandlers);

		bool AreUrlFilesForFeatureInstanceCached(FeatureData featureInstance, IFeatureTypeHandler featureHandler);

		void DeleteUnusedUrlFiles(List<FeatureData> featureInstances, IFeatureTypeHandler featureHandler);

		Texture2D LoadTextureFromCache(IFeatureTypeHandler handler, string url);
	}
}
