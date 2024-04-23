using System;

namespace TactileModules.TactileCloud.AssetBundles
{
	public interface IAssetBundleSystem
	{
		AssetBundleManager AssetBundleManager { get; set; }

		IAssetBundleDownloader AssetBundleDownloader { get; set; }

		IAvailableAssetBundles AvailableAssetBundles { get; set; }
	}
}
