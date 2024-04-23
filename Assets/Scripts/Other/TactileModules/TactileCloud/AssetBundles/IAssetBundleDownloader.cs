using System;
using System.Collections;
using Fibers;

namespace TactileModules.TactileCloud.AssetBundles
{
	public interface IAssetBundleDownloader
	{
		bool IsAssetBundleCached(string url);

		IEnumerator DownloadAssetBundle(string url, EnumeratorResult<DownloadResult> result);

		IEnumerator DownloadAssetBundle(string url, EnumeratorResult<DownloadResult> result, Action<float> onProgress);

		DownloadResult LoadAssetBundle(string url);
	}
}
