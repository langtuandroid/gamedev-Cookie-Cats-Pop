using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using UnityEngine;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureTypeUrlFileCaching
	{
		event Action OnUrlFileCached;

		void CacheUrlFilesForFeatureInstance(FeatureData featureInstance);

		bool AreUrlFilesForFeatureCached(FeatureData featureInstance);

		void DeleteUnusedUrlFiles(List<FeatureData> featureInstances);

		Texture2D LoadTextureFromCache(string url);
	}
}
