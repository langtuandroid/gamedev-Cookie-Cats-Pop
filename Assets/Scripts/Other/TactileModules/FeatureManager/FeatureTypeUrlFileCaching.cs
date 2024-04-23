using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.UrlCaching.Caching;
using UnityEngine;

namespace TactileModules.FeatureManager
{
	public class FeatureTypeUrlFileCaching : IFeatureTypeUrlFileCaching
	{
		public FeatureTypeUrlFileCaching(IFeatureUrlFileHandler featureUrlFileHandler, IUrlCacher urlCacher)
		{
			this.featureUrlFileHandler = featureUrlFileHandler;
			this.urlCacher = urlCacher;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnUrlFileCached;



		public void CacheUrlFilesForFeatureInstance(FeatureData featureInstance)
		{
			foreach (string text in this.featureUrlFileHandler.GetUrlsToCache(featureInstance))
			{
				if (!this.urlCacher.IsCached(text))
				{
					if (!this.urlsToDownload.Contains(text))
					{
						this.urlsToDownload.Add(text);
					}
				}
			}
			if (this.downloadFiber.IsTerminated)
			{
				this.downloadFiber.Start(this.CacheUrlFiles());
			}
		}

		private IEnumerator CacheUrlFiles()
		{
			while (this.urlsToDownload.Count > 0)
			{
				string url = this.urlsToDownload[0];
				EnumeratorResult<bool> result = new EnumeratorResult<bool>();
				yield return this.urlCacher.Cache(url, result);
				this.OnUrlFileCached();
				this.urlsToDownload.Remove(url);
			}
			yield break;
		}

		public bool AreUrlFilesForFeatureCached(FeatureData featureInstance)
		{
			List<string> urlsToCache = this.featureUrlFileHandler.GetUrlsToCache(featureInstance);
			foreach (string url in urlsToCache)
			{
				if (!this.urlCacher.IsCached(url))
				{
					return false;
				}
			}
			return true;
		}

		public void DeleteUnusedUrlFiles(List<FeatureData> featureInstances)
		{
			List<string> filesInUse = this.GetFilesInUse(featureInstances);
			List<string> allCached = this.urlCacher.GetAllCached();
			foreach (string text in allCached)
			{
				if (!filesInUse.Contains(text))
				{
					this.urlCacher.Delete(text);
				}
			}
		}

		private List<string> GetFilesInUse(List<FeatureData> featureInstances)
		{
			List<string> list = new List<string>();
			foreach (FeatureData featureData in featureInstances)
			{
				List<string> urlsToCache = this.featureUrlFileHandler.GetUrlsToCache(featureData);
				foreach (string url in urlsToCache)
				{
					string cachePath = this.urlCacher.GetCachePath(url);
					if (!list.Contains(cachePath))
					{
						list.Add(cachePath);
					}
				}
			}
			return list;
		}

		public Texture2D LoadTextureFromCache(string url)
		{
			return this.urlCacher.LoadTextureFromCache(url);
		}

		private readonly IUrlCacher urlCacher;

		private readonly IFeatureUrlFileHandler featureUrlFileHandler;

		private readonly List<string> urlsToDownload = new List<string>();

		private readonly Fiber downloadFiber = new Fiber();
	}
}
