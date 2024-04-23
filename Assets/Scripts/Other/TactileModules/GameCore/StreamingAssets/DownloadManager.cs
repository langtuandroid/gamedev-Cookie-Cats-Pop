using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

namespace TactileModules.GameCore.StreamingAssets
{
	public class DownloadManager
	{
		public DownloadManager(AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader)
		{
			this.assetBundleManager = assetBundleManager;
			this.assetBundleDownloader = assetBundleDownloader;
		}

		public DownloadManager.LoadedAssetBundle Load(string id)
		{
			DownloadManager.LoadedAssetBundle loadedAssetBundle;
			if (!this.loadedAssetBundles.TryGetValue(id, out loadedAssetBundle))
			{
				loadedAssetBundle = new DownloadManager.LoadedAssetBundle
				{
					Id = id,
					RefCount = 1
				};
				this.loadedAssetBundles.Add(id, loadedAssetBundle);
				FiberCtrl.Pool.Run(this.DownloadAssetBundle(loadedAssetBundle), true);
			}
			else
			{
				loadedAssetBundle.RefCount++;
			}
			return loadedAssetBundle;
		}

		public void Unload(DownloadManager.LoadedAssetBundle bundle)
		{
			bundle.RefCount--;
			if (bundle.RefCount == 0)
			{
				this.loadedAssetBundles.Remove(bundle.Id);
				if (bundle.AssetBundle != null && bundle.DownloadCompleted)
				{
					bundle.AssetBundle.Unload(true);
				}
			}
		}

		private IEnumerator DownloadAssetBundle(DownloadManager.LoadedAssetBundle bundle)
		{
			while (this.numDownloadsInProgress > 3)
			{
				yield return null;
			}
			this.numDownloadsInProgress++;
			bundle.DownloadCompleted = false;
			yield return new Fiber.OnExit(delegate()
			{
				this.numDownloadsInProgress--;
				bundle.DownloadCompleted = true;
				if (bundle.RefCount == 0 && bundle.AssetBundle != null)
				{
					bundle.AssetBundle.Unload(true);
				}
			});
			yield return this.DownloadRemoteAsset(bundle);
			yield break;
		}

		private IEnumerator DownloadRemoteAsset(DownloadManager.LoadedAssetBundle bundle)
		{
			AssetBundleInfo info;
			if (!this.assetBundleManager.AvailableAssetBundles.TryGetValue(bundle.Id, out info))
			{
				yield break;
			}
			WWW www = DownloadManager.LoadFromCacheOrDownload(info.URL, 0);
			yield return www;
			if (www.error != null)
			{
				yield break;
			}
			AssetBundle assetBundle = www.assetBundle;
			if (assetBundle != null)
			{
				bundle.AssetBundle = assetBundle;
				bundle.InvokeDownloadComplete();
			}
			yield break;
		}

		private static WWW LoadFromCacheOrDownload(string url, int version)
		{
			return WWW.LoadFromCacheOrDownload(url, version);
		}

		private const int MAX_SIMULTANEOUS_DOWNLOADS = 3;

		private readonly Dictionary<string, DownloadManager.LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, DownloadManager.LoadedAssetBundle>();

		private readonly AssetBundleManager assetBundleManager;

		private readonly IAssetBundleDownloader assetBundleDownloader;

		private int numDownloadsInProgress;

		public class LoadedAssetBundle
		{
			public string Id { get; set; }

			public AssetBundle AssetBundle { get; set; }

			public int RefCount { get; set; }

			//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action DownloadComplete;

			public bool DownloadCompleted { get; set; }

			public void InvokeDownloadComplete()
			{
				if (this.DownloadComplete != null)
				{
					this.DownloadComplete();
				}
			}
		}
	}
}
