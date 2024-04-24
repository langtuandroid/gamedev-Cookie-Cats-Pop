using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

public class MapAssetBundleManager
{
	public MapAssetBundleManager(AssetBundleManager assetBundleManager, IAssetBundleDownloader assetBundleDownloader)
	{
		this.assetBundleManager = assetBundleManager;
		this.assetBundleDownloader = assetBundleDownloader;
		MapAssetBundleManager.Instance = this;
	}

	public static MapAssetBundleManager Instance { get; private set; }
	
	public static MapQuadrant GetMapImageQuadrant(string imageName)
	{
		if (imageName.Contains("L1"))
		{
			return MapQuadrant.LeftBottom;
		}
		if (imageName.Contains("L0"))
		{
			return MapQuadrant.LeftTop;
		}
		if (imageName.Contains("R1"))
		{
			return MapQuadrant.RightBottom;
		}
		if (imageName.Contains("R0"))
		{
			return MapQuadrant.RightTop;
		}
		return MapQuadrant.Center;
	}

	public void AcquireAssetBundle(string prefabName)
	{
		this.AssetBundleElementRefCountChanged(prefabName, 1);
	}

	public void ReleaseAssetBundle(string prefabName)
	{
		this.AssetBundleElementRefCountChanged(prefabName, -1);
	}

	private void AssetBundleElementRefCountChanged(string prefabName, int change)
	{
		if (this.uniqueAssetBundleElementPrefabs.ContainsKey(prefabName))
		{
			this.uniqueAssetBundleElementPrefabs[prefabName].refCount += change;
			if (this.uniqueAssetBundleElementPrefabs[prefabName].refCount == 0)
			{
				this.uniqueAssetBundleElementPrefabs[prefabName].assetBundle.Unload(true);
				this.uniqueAssetBundleElementPrefabs.Remove(prefabName);
			}
		}
	}

	public bool IsAssetBundleElementCached(string assetBundleName)
	{
		Dictionary<string, TactileModules.TactileCloud.AssetBundles.AssetBundleInfo> availableAssetBundles = this.assetBundleManager.AvailableAssetBundles;
		if (!availableAssetBundles.ContainsKey(assetBundleName))
		{
			return false;
		}
		string url = availableAssetBundles[assetBundleName].URL;
		return Caching.IsVersionCached(url, 0);
	}

	public IEnumerator DownloadAssetBundleElement(string assetBundleName, Action<object, AssetBundle> callback)
	{
		Dictionary<string, TactileModules.TactileCloud.AssetBundles.AssetBundleInfo> availableAssetBundles = this.assetBundleManager.AvailableAssetBundles;
		if (!availableAssetBundles.ContainsKey(assetBundleName))
		{
			callback("MapViewElement Assetbundle not present in available asset bundles", null);
			yield break;
		}
		string url = availableAssetBundles[assetBundleName].URL;
		yield return new Fiber.OnTerminate(delegate()
		{
			callback = null;
		});
		while (this.mapViewElementRequestInProgress)
		{
			yield return null;
		}
		if (this.uniqueAssetBundleElementPrefabs.ContainsKey(assetBundleName) && callback != null)
		{
			callback(null, this.uniqueAssetBundleElementPrefabs[assetBundleName].assetBundle);
			yield break;
		}
		bool internalLoadComplete = false;
		this.mapViewElementRequestInProgress = true;
		FiberCtrl.Pool.Run(this.DownloadAssetInternal(url, delegate(object err, AssetBundle assetBundle)
		{
			internalLoadComplete = true;
			if (callback != null)
			{
				if (assetBundle != null && !this.uniqueAssetBundleElementPrefabs.ContainsKey(assetBundleName))
				{
					MapAssetBundleManager.AssetBundleInfo assetBundleInfo = new MapAssetBundleManager.AssetBundleInfo();
					assetBundleInfo.assetBundle = assetBundle;
					assetBundleInfo.refCount = 0;
					this.uniqueAssetBundleElementPrefabs.Add(assetBundleName, assetBundleInfo);
				}
				callback(err, assetBundle);
			}
			else if (assetBundle != null)
			{
				assetBundle.Unload(true);
			}
			this.mapViewElementRequestInProgress = false;
		}), true);
		while (!internalLoadComplete)
		{
			yield return null;
		}
		yield break;
	}

	public IEnumerator DownloadBigMapSegment(string imageName, Action<object, AssetBundle> callback)
	{
		Dictionary<string, TactileModules.TactileCloud.AssetBundles.AssetBundleInfo> availableAssetBundles = this.assetBundleManager.AvailableAssetBundles;
		if (!availableAssetBundles.ContainsKey(imageName))
		{
			callback("Big map segment not present in available asset bundles", null);
			yield break;
		}
		string url = availableAssetBundles[imageName].URL;
		yield return new Fiber.OnTerminate(delegate()
		{
			callback = null;
		});
		while (this.bigMapSegmentRequestInProgress)
		{
			yield return null;
		}
		bool internalLoadComplete = false;
		this.bigMapSegmentRequestInProgress = true;
		FiberCtrl.Pool.Run(this.DownloadAssetInternal(url, delegate(object err, AssetBundle assetBundle)
		{
			internalLoadComplete = true;
			if (callback != null)
			{
				callback(err, assetBundle);
			}
			else if (assetBundle != null)
			{
				assetBundle.Unload(true);
			}
			this.bigMapSegmentRequestInProgress = false;
		}), true);
		while (!internalLoadComplete)
		{
			yield return null;
		}
		yield break;
	}

	private IEnumerator DownloadAssetInternal(string url, Action<object, AssetBundle> callback)
	{
		EnumeratorResult<DownloadResult> result = new EnumeratorResult<DownloadResult>();
		yield return this.assetBundleDownloader.DownloadAssetBundle(url, result);
		if (!result.value.Success)
		{
			callback(result.value.error, null);
			yield break;
		}
		AssetBundle assetBundle = result.value.assetBundle;
		callback(null, assetBundle);
		yield break;
	}

	private bool smallMapRequestInProgress;

	private bool bigMapSegmentRequestInProgress;

	private bool mapViewElementRequestInProgress;

	private AssetBundleManager assetBundleManager;

	private readonly IAssetBundleDownloader assetBundleDownloader;

	public const string ASSET_BUNDLE_ELEMENT_PREFIX = "assetBundleElement_";

	private Dictionary<string, MapAssetBundleManager.AssetBundleInfo> uniqueAssetBundleElementPrefabs = new Dictionary<string, MapAssetBundleManager.AssetBundleInfo>();

	private class AssetBundleInfo
	{
		public AssetBundle assetBundle;

		public int refCount;
	}
}
