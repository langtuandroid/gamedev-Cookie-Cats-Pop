using System;
using System.Collections.Generic;

namespace TactileModules.TactileCloud.AssetBundles
{
	public interface IAvailableAssetBundles
	{
		Dictionary<string, AssetBundleInfo> AvailableAssetBundles { get; }

		event Action<Dictionary<string, AssetBundleInfo>> AvailableAssetBundlesUpdated;
	}
}
