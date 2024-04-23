using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.CrossPromotion.TactileHub.Data;
using TactileModules.CrossPromotion.TactileHub.Models;
using TactileModules.TactilePrefs;
using TactileModules.UrlCaching.Caching;

namespace TactileModules.CrossPromotion.Cloud.DataRetrievers
{
	public class GeneralDataRetriever : IGeneralDataRetriever
	{
		public GeneralDataRetriever(ICrossPromotionGeneralDataCloud crossPromotionGeneralDataCloud, ILocalStorageObject<CrossPromotionGeneralData> localStorage, IUrlCacher textureCacher, IHubGameFactory hubGameFactory, IPromotedGameLauncher promotedGameLauncher)
		{
			this.crossPromotionGeneralDataCloud = crossPromotionGeneralDataCloud;
			this.localStorage = localStorage;
			this.textureCacher = textureCacher;
			this.hubGameFactory = hubGameFactory;
			this.promotedGameLauncher = promotedGameLauncher;
			this.RequestData();
		}

		private void RequestData()
		{
			if (this.requestFiber.IsTerminated)
			{
				this.requestFiber.Start(this.RequestDataCoroutine());
			}
		}

		private IEnumerator RequestDataCoroutine()
		{
			EnumeratorResult<CrossPromotionGeneralData> result = new EnumeratorResult<CrossPromotionGeneralData>();
			yield return this.crossPromotionGeneralDataCloud.GetCrossPromotionGeneralData(result);
			if (result.value != null)
			{
				this.SaveGeneralData(result.value);
				this.CacheAllUncachedHubIcons();
			}
			yield break;
		}

		private void SaveGeneralData(CrossPromotionGeneralData generalData)
		{
			this.cachedStorage = generalData;
			this.localStorage.Save(generalData);
		}

		public void CacheAllUncachedHubIcons()
		{
			CrossPromotionGeneralData crossPromotionGeneralData = this.localStorage.Load();
			List<IEnumerator> list = new List<IEnumerator>();
			foreach (HubGameData hubGameData in crossPromotionGeneralData.HubGames)
			{
				if (!this.textureCacher.IsCached(hubGameData.IconUrl))
				{
					list.Add(this.textureCacher.Cache(hubGameData.IconUrl, new EnumeratorResult<bool>()));
				}
			}
			this.cachingFiber.Start(FiberHelper.RunSerial(list));
		}

		public CrossPromotionGeneralData GetGeneralData()
		{
			if (this.cachedStorage == null)
			{
				this.cachedStorage = this.localStorage.Load();
			}
			return this.cachedStorage;
		}

		public List<IHubGame> GetAllCachedHubGames()
		{
			CrossPromotionGeneralData crossPromotionGeneralData = this.localStorage.Load();
			List<IHubGame> list = new List<IHubGame>();
			foreach (HubGameData hubGameData in crossPromotionGeneralData.HubGames)
			{
				if (this.textureCacher.IsCached(hubGameData.IconUrl))
				{
					string cachePath = this.textureCacher.GetCachePath(hubGameData.IconUrl);
					IHubGame item = this.hubGameFactory.Create(hubGameData, cachePath);
					list.Add(item);
				}
			}
			return list;
		}

		public string[] GetAllInstalledGames()
		{
			List<string> list = new List<string>();
			List<CrossPromotionAvailableApp> availableApps = this.GetGeneralData().AvailableApps;
			foreach (CrossPromotionAvailableApp crossPromotionAvailableApp in availableApps)
			{
				if (this.promotedGameLauncher.IsGameInstalled(crossPromotionAvailableApp))
				{
					list.Add(crossPromotionAvailableApp.PackageName);
				}
			}
			return list.ToArray();
		}

		public int GetMaxAdsPerSession(AdType type)
		{
			CrossPromotionClientConfiguration crossPromotionClientConfiguration = this.GetGeneralData().CrossPromotionClientConfiguration;
			if (type == AdType.Interstitial)
			{
				return crossPromotionClientConfiguration.MaxInterstitialAdsPerSession;
			}
			return crossPromotionClientConfiguration.MaxRewardedAdsPerSession;
		}

		private readonly ICrossPromotionGeneralDataCloud crossPromotionGeneralDataCloud;

		private readonly ILocalStorageObject<CrossPromotionGeneralData> localStorage;

		private readonly IUrlCacher textureCacher;

		private readonly IHubGameFactory hubGameFactory;

		private readonly IPromotedGameLauncher promotedGameLauncher;

		private readonly Fiber requestFiber = new Fiber();

		private readonly Fiber cachingFiber = new Fiber();

		private CrossPromotionGeneralData cachedStorage;
	}
}
