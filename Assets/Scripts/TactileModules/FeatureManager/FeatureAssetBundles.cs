using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.TactileCloud.AssetBundles;

namespace TactileModules.FeatureManager
{
	public class FeatureAssetBundles : IFeatureAssetBundles
	{
		public FeatureAssetBundles(IAssetBundleDownloader assetBundleDownloader, IAvailableAssetBundles availableAssetBundles)
		{
			this.assetBundleDownloader = assetBundleDownloader;
			this.availableAssetBundles = availableAssetBundles;
			this.availableAssetBundles.AvailableAssetBundlesUpdated += this.OnAvailableAssetBundlesUpdated;
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action AvailableAssetBundlesUpdated;

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnAssetBundleDownloaded;



		private void OnAvailableAssetBundlesUpdated(Dictionary<string, AssetBundleInfo> data)
		{
			if (this.AvailableAssetBundlesUpdated != null)
			{
				this.AvailableAssetBundlesUpdated();
			}
		}

		public void DownloadAssetBundle(string assetBundleName)
		{
			this.assetBundlesToDownload.Add(assetBundleName);
			if (this.downloadFiber.IsTerminated)
			{
				this.downloadFiber.Start(this.DownloadAndUnloadAssetBundles());
			}
		}

		public bool IsAssetBundleDownloaded(string assetBundleName)
		{
			string urlFromAssetBundleName = this.GetUrlFromAssetBundleName(assetBundleName);
			return this.assetBundleDownloader.IsAssetBundleCached(urlFromAssetBundleName);
		}

		public IEnumerator GetAssetBundle(string assetBundleName, EnumeratorResult<DownloadResult> result)
		{
			if (!this.IsAssetBundleAvailable(assetBundleName))
			{
				string error = string.Format("AssetBundle : '{0}' is not available.", assetBundleName);
				result.value = new DownloadResult(null, error);
				yield break;
			}
			string url = this.GetUrlFromAssetBundleName(assetBundleName);
			if (string.IsNullOrEmpty(url))
			{
				string error2 = string.Format("Url for AssetBundle : '{0}' is Null or Empty.", assetBundleName);
				result.value = new DownloadResult(null, error2);
				yield break;
			}
			yield return this.assetBundleDownloader.DownloadAssetBundle(url, result);
			yield break;
		}

		private IEnumerator DownloadAndUnloadAssetBundles()
		{
			while (this.assetBundlesToDownload.Count > 0)
			{
				string assetBundleName = this.assetBundlesToDownload[0];
				yield return this.DownloadAndUnloadAssetBundle(assetBundleName);
				this.OnAssetBundleDownloaded();
				this.assetBundlesToDownload.Remove(assetBundleName);
			}
			yield break;
		}

		private IEnumerator DownloadAndUnloadAssetBundle(string assetBundleName)
		{
			if (this.IsAssetBundleDownloaded(assetBundleName))
			{
				yield break;
			}
			EnumeratorResult<DownloadResult> result = new EnumeratorResult<DownloadResult>();
			yield return this.GetAssetBundle(assetBundleName, result);
			if (result.value.Success)
			{
				result.value.assetBundle.Unload(true);
			}
			yield break;
		}

		private bool IsAssetBundleAvailable(string assetBundleName)
		{
			return this.availableAssetBundles.AvailableAssetBundles.ContainsKey(assetBundleName);
		}

		private string GetUrlFromAssetBundleName(string assetBundleName)
		{
			if (!this.availableAssetBundles.AvailableAssetBundles.ContainsKey(assetBundleName))
			{
				return null;
			}
			return Uri.EscapeUriString(this.availableAssetBundles.AvailableAssetBundles[assetBundleName].URL);
		}

		public void DownloadAssetBundlesForFeatures(List<FeatureData> features)
		{
			foreach (FeatureData featureData in features)
			{
				IFeatureTypeHandler featureTypeHandler = featureData.FeatureHandler();
				IFeatureAssetBundleHandler featureAssetBundleHandler = featureTypeHandler as IFeatureAssetBundleHandler;
				if (featureAssetBundleHandler != null)
				{
					foreach (string assetBundleName in featureAssetBundleHandler.GetAssetBundles(featureData))
					{
						if (!this.IsAssetBundleDownloaded(assetBundleName))
						{
							this.DownloadAssetBundle(assetBundleName);
						}
					}
				}
			}
		}

		public bool AreAssetBundlesForFeatureDownloaded(FeatureData featureData)
		{
			IFeatureTypeHandler featureTypeHandler = featureData.FeatureHandler();
			IFeatureAssetBundleHandler featureAssetBundleHandler = featureTypeHandler as IFeatureAssetBundleHandler;
			if (featureAssetBundleHandler != null)
			{
				foreach (string assetBundleName in featureAssetBundleHandler.GetAssetBundles(featureData))
				{
					if (!this.IsAssetBundleDownloaded(assetBundleName))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public DownloadResult LoadAssetBundleFromCache(string assetBundleName)
		{
			if (!this.IsAssetBundleDownloaded(assetBundleName))
			{
				return new DownloadResult(null, string.Format("AssetBundle '{0}' was not downloaded.", assetBundleName));
			}
			string urlFromAssetBundleName = this.GetUrlFromAssetBundleName(assetBundleName);
			if (string.IsNullOrEmpty(urlFromAssetBundleName))
			{
				string error = string.Format("Url for AssetBundle : '{0}' is Null or Empty.", assetBundleName);
				return new DownloadResult(null, error);
			}
			return this.assetBundleDownloader.LoadAssetBundle(urlFromAssetBundleName);
		}

		private readonly IAssetBundleDownloader assetBundleDownloader;

		private readonly IAvailableAssetBundles availableAssetBundles;

		private readonly List<string> assetBundlesToDownload = new List<string>();

		private readonly Fiber downloadFiber = new Fiber();
	}
}
