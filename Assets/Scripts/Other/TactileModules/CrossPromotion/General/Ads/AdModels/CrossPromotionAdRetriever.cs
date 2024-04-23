using System;
using System.Collections;
using Fibers;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.RuntimeTools;
using TactileModules.TactilePrefs;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public class CrossPromotionAdRetriever : ICrossPromotionAdRetriever
	{
		public CrossPromotionAdRetriever(ICrossPromotionAdFactory crossPromotionAdFactory, IAdSession session, ICrossPromotionAdCloud cloud, ILocalStorageObject<CrossPromotionAdRetrieverData> storage, IGeneralDataRetriever generalDataRetriever, ICrossPromotionAnalyticsEventFactory analyticsEventFactory, ITactileDateTime dateTimeGetter, IUserProgressProvider userProgressProvider)
		{
			this.crossPromotionAdFactory = crossPromotionAdFactory;
			this.session = session;
			this.cloud = cloud;
			this.storage = storage;
			this.generalDataRetriever = generalDataRetriever;
			this.analyticsEventFactory = analyticsEventFactory;
			this.dateTimeGetter = dateTimeGetter;
			this.userProgressProvider = userProgressProvider;
			this.LoadPromotionAd();
		}

		private void LoadPromotionAd()
		{
			if (this.cachedAdRetrieverData == null)
			{
				this.cachedAdRetrieverData = this.storage.Load();
			}
			if (this.cachedAdRetrieverData != null && this.cachedAdRetrieverData.AdMetaData != null)
			{
				this.crossPromotionAd = this.crossPromotionAdFactory.Create(this.cachedAdRetrieverData.AdMetaData, this.cachedAdRetrieverData.RequestTimestamp, this.session, this.analyticsEventFactory);
				this.crossPromotionAd.EnsureIsCached();
			}
		}

		public ICrossPromotionAd GetPromotion()
		{
			return this.crossPromotionAd;
		}

		public ICrossPromotionAd GetPresentablePromotion()
		{
			if (!this.CanPresentPromotion())
			{
				return null;
			}
			return this.crossPromotionAd;
		}

		public bool IsRequesting()
		{
			return !this.cloudFiber.IsTerminated;
		}

		private bool CanPresentPromotion()
		{
			return this.crossPromotionAd != null && this.crossPromotionAd.IsCached() && !this.crossPromotionAd.HasExpired() && this.crossPromotionAd.CanShowInThisSession();
		}

		public void RequestNewPromotion()
		{
			if (this.cloudFiber.IsTerminated)
			{
				this.crossPromotionAd = null;
				this.cloudFiber.Start(this.RefreshCrossPromotionsInternal());
			}
		}

		private IEnumerator RefreshCrossPromotionsInternal()
		{
			string[] installedApps = this.generalDataRetriever.GetAllInstalledGames();
			EnumeratorResult<CrossPromotionAdMetaData> result = new EnumeratorResult<CrossPromotionAdMetaData>();
			yield return this.cloud.GetCrossPromotionAd(installedApps, this.userProgressProvider.GetUserProgress(), result);
			if (result.value != null)
			{
				this.SetPromotion(result.value);
			}
			yield break;
		}

		private void SetPromotion(CrossPromotionAdMetaData adMetaData)
		{
			CrossPromotionAdRetrieverData crossPromotionAdRetrieverData = this.storage.Load();
			crossPromotionAdRetrieverData.AdMetaData = adMetaData;
			crossPromotionAdRetrieverData.RequestTimestamp = this.dateTimeGetter.UtcNow;
			this.storage.Save(crossPromotionAdRetrieverData);
			this.cachedAdRetrieverData = crossPromotionAdRetrieverData;
			this.LoadPromotionAd();
		}

		private readonly ICrossPromotionAdFactory crossPromotionAdFactory;

		private readonly IAdSession session;

		private readonly ICrossPromotionAdCloud cloud;

		private readonly ILocalStorageObject<CrossPromotionAdRetrieverData> storage;

		private readonly IGeneralDataRetriever generalDataRetriever;

		private readonly ICrossPromotionAnalyticsEventFactory analyticsEventFactory;

		private readonly ITactileDateTime dateTimeGetter;

		private readonly IUserProgressProvider userProgressProvider;

		private ICrossPromotionAd crossPromotionAd;

		private readonly Fiber cloudFiber = new Fiber();

		private CrossPromotionAdRetrieverData cachedAdRetrieverData;
	}
}
