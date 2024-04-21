using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.TactileCloud.AssetBundles;
using UnityEngine;

public class TextureStreamerAssetBundleManager
{
	private TextureStreamerAssetBundleManager(TextureStreamerAssetBundleManager.IDataProvider dataProvider, IAssetBundleDownloader assetBundleDownloader)
	{
		this.assetBundleDownloader = assetBundleDownloader;
		this.DataProvider = dataProvider;
		FiberCtrl.Pool.Run(this.UpdateEndless(), false);
	}

	public static TextureStreamerAssetBundleManager Instance { get; private set; }

	public TextureStreamerAssetBundleManager.IDataProvider DataProvider { get; private set; }

	public static TextureStreamerAssetBundleManager CreateInstance(TextureStreamerAssetBundleManager.IDataProvider dataProvider, IAssetBundleDownloader assetBundleDownloader)
	{
		TextureStreamerAssetBundleManager.Instance = new TextureStreamerAssetBundleManager(dataProvider, assetBundleDownloader);
		return TextureStreamerAssetBundleManager.Instance;
	}

	private static void Log(string message)
	{
		
	}

	private static void LogError(string message)
	{

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
		if (this.cachedAssetBundles.ContainsKey(prefabName))
		{
			this.cachedAssetBundles[prefabName].refCount += change;
			if (this.cachedAssetBundles[prefabName].refCount == 0)
			{
				this.cachedAssetBundles[prefabName].assetBundle.Unload(true);
				this.cachedAssetBundles.Remove(prefabName);
			}
		}
		else
		{
			TextureStreamerAssetBundleManager.LogError(string.Concat(new object[]
			{
				"TextureStreamerAssetBundleManager/AssetBundleElementRefCountChanged by value=",
				change,
				" This was not supposed to happen. please investigate. (assetbundle=",
				prefabName,
				" should exist at this point)"
			}));
		}
	}

	public IEnumerator DownloadAssetBundle(string assetBundleName, Action<object, AssetBundle> callback)
	{
		Dictionary<string, TactileModules.TactileCloud.AssetBundles.AssetBundleInfo> availableAssetBundles = this.DataProvider.GetAssetBundleManager().AvailableAssetBundles;
		if (!availableAssetBundles.ContainsKey(assetBundleName))
		{
			TextureStreamerAssetBundleManager.LogError("TextureStreamerAssetBundleManager/DownloadAssetBundle Assetbundle not present in available asset bundles. assetBundleName=" + assetBundleName);
			callback("Assetbundle not present in available asset bundles", null);
			yield break;
		}
		string url = Uri.EscapeUriString(availableAssetBundles[assetBundleName].URL);
		yield return new Fiber.OnTerminate(delegate()
		{
			callback = null;
		});
		while (this.requestInProgress)
		{
			yield return null;
		}
		if (this.cachedAssetBundles.ContainsKey(assetBundleName) && callback != null)
		{
			TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadAssetBundle Assetbundle is already in use - so we use this");
			callback(null, this.cachedAssetBundles[assetBundleName].assetBundle);
			yield break;
		}
		bool internalLoadComplete = false;
		this.requestInProgress = true;
		FiberCtrl.Pool.Run(this.DownloadAssetInternal(url, delegate(object err, AssetBundle assetBundle)
		{
			internalLoadComplete = true;
			if (callback != null)
			{
				if (assetBundle != null)
				{
					if (!this.cachedAssetBundles.ContainsKey(assetBundleName))
					{
						TextureStreamerAssetBundleManager.AssetBundleInfo assetBundleInfo = new TextureStreamerAssetBundleManager.AssetBundleInfo();
						assetBundleInfo.assetBundle = assetBundle;
						assetBundleInfo.refCount = 0;
						this.cachedAssetBundles.Add(assetBundleName, assetBundleInfo);
					}
					else
					{
						TextureStreamerAssetBundleManager.LogError("TextureStreamerAssetBundleManager/DownloadAssetBundle This was not supposed to happen. please investigate. (dont load assetbunlde if it already exists)");
					}
				}
				callback(err, assetBundle);
			}
			else
			{
				TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadAssetBundle No callback - we unload bundle : " + url);
				if (assetBundle != null)
				{
					assetBundle.Unload(true);
				}
			}
			this.requestInProgress = false;
		}), false);
		while (!internalLoadComplete)
		{
			yield return null;
		}
		yield break;
	}

	private IEnumerator DownloadAssetInternal(string url, Action<object, AssetBundle> callback)
	{
		TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadAssetInternal Loading bundle " + url);
		EnumeratorResult<DownloadResult> result = new EnumeratorResult<DownloadResult>();
		yield return this.assetBundleDownloader.DownloadAssetBundle(url, result);
		if (!result.value.Success)
		{
			string text = "TextureStreamerAssetBundleManager/DownloadAssetInternal. " + result.value.error;
			TextureStreamerAssetBundleManager.LogError(text);
			callback(text, null);
		}
		TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadAssetInternal Fetching successful : bundle " + url);
		callback(null, result.value.assetBundle);
		yield break;
	}

	private IEnumerator UpdateEndless()
	{
		for (;;)
		{
			while (!this.DataProvider.PendingDoUpdateAssetBundles)
			{
				yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			}
			this.DataProvider.PendingDoUpdateAssetBundles = false;
			yield return this.DownloadNonCachedAssetBundles(this.DataProvider.FarthestCompletedLevelHumanNumber);
		}
		yield break;
	}

	private IEnumerator DownloadNonCachedAssetBundles(int levelNr)
	{
		TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadNonCachedAssetBundles");
		if (this.updateAssetBundlesInProgress)
		{
			yield break;
		}
		yield return new Fiber.OnExit(delegate()
		{
			this.updateAssetBundlesInProgress = false;
		});
		this.updateAssetBundlesInProgress = true;
		Dictionary<string, TactileModules.TactileCloud.AssetBundles.AssetBundleInfo> availableAssetBundles = this.DataProvider.GetAssetBundleManager().AvailableAssetBundles;
		foreach (TextureStreamerSettings.TextureStreamerElement a in SingletonAsset<TextureStreamerSettings>.Instance.assetBundlesToCacheAtLevelUnlock)
		{
			if (levelNr < a.cacheAtLevelNr)
			{
				TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadNonCachedAssetBundles : Assetbundle should not be downloaded yet!.  assetBundleName=" + a.assetBundleName);
			}
			else if (!availableAssetBundles.ContainsKey(a.assetBundleName))
			{
				TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadNonCachedAssetBundles : Assetbundle not present in available asset bundles. assetBundleName=" + a.assetBundleName);
			}
			else
			{
				string url = availableAssetBundles[a.assetBundleName].URL;
				if (Caching.IsVersionCached(url, 0))
				{
					TextureStreamerAssetBundleManager.Log("TextureStreamerAssetBundleManager/DownloadNonCachedAssetBundles : Assetbundle is already cached. AssetBundleName=" + a.assetBundleName);
				}
				else
				{
					yield return this.DownloadAssetBundle(a.assetBundleName, null);
				}
			}
		}
		yield break;
	}

	public bool IsAssetbundelsAvailable()
	{
		return this.DataProvider.GetAssetBundleManager().AvailableAssetBundles.Count > 0;
	}

	private readonly IAssetBundleDownloader assetBundleDownloader;

	private Dictionary<string, TextureStreamerAssetBundleManager.AssetBundleInfo> cachedAssetBundles = new Dictionary<string, TextureStreamerAssetBundleManager.AssetBundleInfo>();

	private bool requestInProgress;

	private bool updateAssetBundlesInProgress;

	public interface IDataProvider
	{
		AssetBundleManager GetAssetBundleManager();

		int FarthestCompletedLevelHumanNumber { get; }

		bool PendingDoUpdateAssetBundles { get; set; }
	}

	private class AssetBundleInfo
	{
		public AssetBundle assetBundle;

		public int refCount;
	}
}
