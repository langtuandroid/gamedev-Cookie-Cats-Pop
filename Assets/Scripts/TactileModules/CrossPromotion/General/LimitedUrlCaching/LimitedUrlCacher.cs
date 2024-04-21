using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.CrossPromotion.General.LimitedUrlCaching.Data;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.CrossPromotion.General.LimitedUrlCaching
{
	public class LimitedUrlCacher : ILimitedUrlCacher
	{
		public LimitedUrlCacher(IUrlCacher urlCacher, int cacheLimit, ILocalStorageObject<LimitedUrlCacherData> storage, ITactileDateTime dateTimeGetter)
		{
			this.urlCacher = urlCacher;
			this.cacheLimit = cacheLimit;
			this.storage = storage;
			this.dateTimeGetter = dateTimeGetter;
		}

		public void EnsureAssetIsCached(string url)
		{
			if (!this.IsCached(url))
			{
				if (!this.cacheFiber.IsTerminated)
				{
					return;
				}
				this.cacheFiber.Start(this.EnsureAssetIsCachedInternal(url));
			}
			else
			{
				this.UpdateTimeStampForCache(url);
			}
		}

		private IEnumerator EnsureAssetIsCachedInternal(string url)
		{
			if (this.ShouldRemoveOldestCache())
			{
				this.RemoveOldestUsedCache();
			}
			EnumeratorResult<bool> success = new EnumeratorResult<bool>();
			yield return this.urlCacher.Cache(url, success);
			if (success.value)
			{
				this.UpdateTimeStampForCache(url);
			}
			yield break;
		}

		public bool IsCached(string url)
		{
			return this.urlCacher.IsCached(url);
		}

		public string GetCachePath(string url)
		{
			return this.urlCacher.GetCachePath(url);
		}

		private void UpdateTimeStampForCache(string url)
		{
			LimitedUrlCacherData limitedUrlCacherData = this.LoadStorage();
			Dictionary<string, DateTime> cacheTimeStamps = limitedUrlCacherData.CacheTimeStamps;
			if (cacheTimeStamps.ContainsKey(url))
			{
				cacheTimeStamps[url] = this.dateTimeGetter.UtcNow;
			}
			else
			{
				cacheTimeStamps.Add(url, this.dateTimeGetter.UtcNow);
			}
			this.SaveStorage(limitedUrlCacherData);
		}

		private bool ShouldRemoveOldestCache()
		{
			LimitedUrlCacherData limitedUrlCacherData = this.LoadStorage();
			Dictionary<string, DateTime> cacheTimeStamps = limitedUrlCacherData.CacheTimeStamps;
			return cacheTimeStamps.Count >= this.cacheLimit;
		}

		private void RemoveOldestUsedCache()
		{
			LimitedUrlCacherData limitedUrlCacherData = this.LoadStorage();
			Dictionary<string, DateTime> cacheTimeStamps = limitedUrlCacherData.CacheTimeStamps;
			string oldestTimeStampUrl = this.GetOldestTimeStampUrl(cacheTimeStamps);
			if (oldestTimeStampUrl != null)
			{
				string cachePath = this.GetCachePath(oldestTimeStampUrl);
				this.urlCacher.Delete(cachePath);
				cacheTimeStamps.Remove(oldestTimeStampUrl);
				this.SaveStorage(limitedUrlCacherData);
			}
		}

		private string GetOldestTimeStampUrl(Dictionary<string, DateTime> cacheTimestamps)
		{
			string text = null;
			foreach (KeyValuePair<string, DateTime> keyValuePair in cacheTimestamps)
			{
				if (text == null || keyValuePair.Value < cacheTimestamps[text])
				{
					text = keyValuePair.Key;
				}
			}
			return text;
		}

		private LimitedUrlCacherData LoadStorage()
		{
			if (this.cachedStorageData == null)
			{
				this.cachedStorageData = this.storage.Load();
			}
			return this.cachedStorageData;
		}

		private void SaveStorage(LimitedUrlCacherData storageData)
		{
			this.cachedStorageData = storageData;
			this.storage.Save(storageData);
		}

		private readonly IUrlCacher urlCacher;

		private readonly int cacheLimit;

		private readonly ILocalStorageObject<LimitedUrlCacherData> storage;

		private readonly Fiber cacheFiber = new Fiber();

		private readonly ITactileDateTime dateTimeGetter;

		private LimitedUrlCacherData cachedStorageData;
	}
}
