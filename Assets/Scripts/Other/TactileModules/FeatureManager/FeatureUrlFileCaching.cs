using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.UrlCaching.Caching;
using UnityEngine;

namespace TactileModules.FeatureManager
{
	public class FeatureUrlFileCaching : IFeatureUrlFileCaching
	{
		public FeatureUrlFileCaching(IFeatureTypeUrlFileCachingFactory featureTypeUrlFileCachingFactory, IUrlCacherFactory urlCacherFactory)
		{
			this.featureTypeUrlFileCachingFactory = featureTypeUrlFileCachingFactory;
			this.urlCacherFactory = urlCacherFactory;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnUrlFileCached;



		public void CacheUrlFilesForFeatures(List<FeatureData> featureInstances, List<IFeatureTypeHandler> featureHandlers)
		{
			foreach (FeatureData featureData in featureInstances)
			{
				IFeatureTypeHandler handler = this.GetHandler(featureData, featureHandlers);
				if (handler != null)
				{
					IFeatureTypeUrlFileCaching urlFileCachingForFeatureType = this.GetUrlFileCachingForFeatureType(handler);
					if (urlFileCachingForFeatureType != null)
					{
						urlFileCachingForFeatureType.CacheUrlFilesForFeatureInstance(featureData);
					}
				}
			}
		}

		public bool AreUrlFilesForFeatureInstanceCached(FeatureData featureInstance, IFeatureTypeHandler featureHandler)
		{
			IFeatureTypeUrlFileCaching urlFileCachingForFeatureType = this.GetUrlFileCachingForFeatureType(featureHandler);
			return urlFileCachingForFeatureType == null || urlFileCachingForFeatureType.AreUrlFilesForFeatureCached(featureInstance);
		}

		public void DeleteUnusedUrlFiles(List<FeatureData> featureInstances, IFeatureTypeHandler featureHandler)
		{
			IFeatureTypeUrlFileCaching urlFileCachingForFeatureType = this.GetUrlFileCachingForFeatureType(featureHandler);
			if (urlFileCachingForFeatureType != null)
			{
				urlFileCachingForFeatureType.DeleteUnusedUrlFiles(featureInstances);
			}
		}

		public Texture2D LoadTextureFromCache(IFeatureTypeHandler handler, string url)
		{
			IFeatureTypeUrlFileCaching urlFileCachingForFeatureType = this.GetUrlFileCachingForFeatureType(handler);
			if (urlFileCachingForFeatureType != null)
			{
				return urlFileCachingForFeatureType.LoadTextureFromCache(url);
			}
			return null;
		}

		private IFeatureTypeHandler GetHandler(FeatureData featureData, List<IFeatureTypeHandler> featureHandlers)
		{
			foreach (IFeatureTypeHandler featureTypeHandler in featureHandlers)
			{
				if (featureTypeHandler.FeatureType == featureData.Type)
				{
					return featureTypeHandler;
				}
			}
			return null;
		}

		private IFeatureTypeUrlFileCaching GetUrlFileCachingForFeatureType(IFeatureTypeHandler handler)
		{
			IFeatureUrlFileHandler featureUrlFileHandler = handler as IFeatureUrlFileHandler;
			if (featureUrlFileHandler == null)
			{
				return null;
			}
			if (!this.featureTypeToUrlFileCaching.ContainsKey(handler.FeatureType))
			{
				IUrlCacher urlCacher = this.urlCacherFactory.Create(handler.FeatureType);
				IFeatureTypeUrlFileCaching featureTypeUrlFileCaching = this.featureTypeUrlFileCachingFactory.Create(featureUrlFileHandler, urlCacher);
				featureTypeUrlFileCaching.OnUrlFileCached += this.HandleUrlFileCached;
				this.featureTypeToUrlFileCaching.Add(handler.FeatureType, featureTypeUrlFileCaching);
			}
			return this.featureTypeToUrlFileCaching[handler.FeatureType];
		}

		private void HandleUrlFileCached()
		{
			this.OnUrlFileCached();
		}

		private readonly IFeatureTypeUrlFileCachingFactory featureTypeUrlFileCachingFactory;

		private readonly IUrlCacherFactory urlCacherFactory;

		private readonly Dictionary<string, IFeatureTypeUrlFileCaching> featureTypeToUrlFileCaching = new Dictionary<string, IFeatureTypeUrlFileCaching>();
	}
}
