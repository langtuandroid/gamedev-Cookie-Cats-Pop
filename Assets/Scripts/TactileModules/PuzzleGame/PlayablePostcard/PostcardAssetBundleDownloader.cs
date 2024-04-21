using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

namespace TactileModules.PuzzleGame.PlayablePostcard
{
	public class PostcardAssetBundleDownloader
	{
		public PostcardAssetBundleDownloader(AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader)
		{
			this.assetBundleManager = assetBundleManager;
			this.assetBundleDownloader = assetBundleDownloader;
		}

		private bool IsLoading
		{
			get
			{
				return !this.loadFiber.IsTerminated;
			}
		}

		public bool IsAssetBundleLoaded(string assetBundleName)
		{
			return this.loadedAssetBundles.ContainsKey(assetBundleName);
		}

		public void LoadAssetBundle(string assetBundleName)
		{
			if (!this.IsAssetBundleLoaded(assetBundleName) && !this.IsLoading)
			{
				this.loadFiber.Start(this.LoadAssetBundleInternal(assetBundleName));
			}
		}

		public void UnloadAssetBundle(string assetBundleName)
		{
			if (this.loadedAssetBundles.ContainsKey(assetBundleName))
			{
				this.loadedAssetBundles[assetBundleName].Unload(true);
				this.loadedAssetBundles.Remove(assetBundleName);
			}
		}

		public AssetBundle GetAssetBundle(string assetBundleName)
		{
			if (this.loadedAssetBundles.ContainsKey(assetBundleName))
			{
				return this.loadedAssetBundles[assetBundleName];
			}
			return null;
		}

		public bool CanAssetBundleBeDownloaded(string assetBundleName)
		{
			return this.assetBundleManager.AvailableAssetBundles.ContainsKey(assetBundleName);
		}

		public bool IsCached(string assetBundleName)
		{
			string urlFromAssetBundleName = this.GetUrlFromAssetBundleName(assetBundleName);
			return Caching.IsVersionCached(urlFromAssetBundleName, 0);
		}

		public string GetUrlFromAssetBundleName(string assetBundleName)
		{
			Dictionary<string, AssetBundleInfo> availableAssetBundles = this.assetBundleManager.AvailableAssetBundles;
			if (!availableAssetBundles.ContainsKey(assetBundleName))
			{
				return string.Empty;
			}
			return Uri.EscapeUriString(availableAssetBundles[assetBundleName].URL);
		}

		private IEnumerator LoadAssetBundleInternal(string assetBundleName)
		{
			string url = this.GetUrlFromAssetBundleName(assetBundleName);
			EnumeratorResult<DownloadResult> result = new EnumeratorResult<DownloadResult>();
			yield return this.assetBundleDownloader.DownloadAssetBundle(url, result);
			if (result.value.Success)
			{
				if (!this.loadedAssetBundles.ContainsKey(assetBundleName))
				{
					this.loadedAssetBundles.Add(assetBundleName, result.value.assetBundle);
				}
			}
			else
			{
				string text = "PostcardAssetBundleDownloader/LoadAssetBundleInternal Error fetching asset bundle: " + url + " error: " + result.value.error;
			}
			yield break;
		}

		private readonly Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();

		private readonly AssetBundleManager assetBundleManager;

		private readonly IAssetBundleDownloader assetBundleDownloader;

		private readonly Fiber loadFiber = new Fiber();
	}
}
