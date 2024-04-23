using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.TactileCloud.AssetBundles;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureAssetBundles
	{
		event Action AvailableAssetBundlesUpdated;

		event Action OnAssetBundleDownloaded;

		void DownloadAssetBundle(string assetBundleName);

		bool IsAssetBundleDownloaded(string assetBundleName);

		IEnumerator GetAssetBundle(string assetBundleName, EnumeratorResult<DownloadResult> result);

		void DownloadAssetBundlesForFeatures(List<FeatureData> features);

		bool AreAssetBundlesForFeatureDownloaded(FeatureData featureData);

		DownloadResult LoadAssetBundleFromCache(string assetBundleName);
	}
}
