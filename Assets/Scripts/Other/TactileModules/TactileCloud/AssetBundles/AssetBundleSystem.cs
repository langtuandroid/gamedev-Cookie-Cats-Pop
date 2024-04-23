using System;

namespace TactileModules.TactileCloud.AssetBundles
{
	public class AssetBundleSystem : IAssetBundleSystem
	{
		public AssetBundleSystem(AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader, IAvailableAssetBundles availableAssetBundles)
		{
			this.AssetBundleManager = assetBundleManager;
			this.AssetBundleDownloader = assetBundleDownloader;
			this.AvailableAssetBundles = availableAssetBundles;
		}

		public AssetBundleManager AssetBundleManager { get; set; }

		public IAssetBundleDownloader AssetBundleDownloader { get; set; }

		public IAvailableAssetBundles AvailableAssetBundles { get; set; }
	}
}
