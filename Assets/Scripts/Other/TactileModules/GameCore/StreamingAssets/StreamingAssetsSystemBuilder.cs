using System;
using TactileModules.GameCore.StreamingAssets.Assets;
using TactileModules.TactileCloud.AssetBundles;

namespace TactileModules.GameCore.StreamingAssets
{
	public static class StreamingAssetsSystemBuilder
	{
		public static void Build(AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader)
		{
			DownloadManager dependencies = new DownloadManager(assetBundleManager, assetBundleDownloader);
			IAssetsModel assetsModel = new AssetsModel();
			StreamingAssetsDependencies streamingAssetsDependencies = assetsModel.StreamingAssetsDependencies;
			streamingAssetsDependencies.SetDependencies(dependencies);
		}
	}
}
