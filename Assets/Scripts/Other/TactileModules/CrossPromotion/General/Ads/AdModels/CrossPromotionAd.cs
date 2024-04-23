using System;
using System.Collections.Generic;
using TactileModules.Ads.Analytics;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.General.LimitedUrlCaching;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.RuntimeTools;
using UnityEngine;

namespace TactileModules.CrossPromotion.General.Ads.AdModels
{
	public class CrossPromotionAd : ICrossPromotionAd
	{
		public CrossPromotionAd(CrossPromotionAdMetaData data, DateTime requestTime, ILimitedUrlCacherRetriever adCacherRetriever, IAdCreativeSelector adCreativeSelector, IPromotedGameLauncher promotedGameLauncher, ITextureLoader textureLoader, IAdSession session, IGeneralDataRetriever generalDataRetriever, ITactileDateTime dateTimeGetter, ICrossPromotionAnalyticsEventFactory analyticsEventFactory, IAnalytics analytics)
		{
			this.data = data;
			this.requestTime = requestTime;
			this.adCacherRetriever = adCacherRetriever;
			this.adCreativeSelector = adCreativeSelector;
			this.promotedGameLauncher = promotedGameLauncher;
			this.textureLoader = textureLoader;
			this.session = session;
			this.generalDataRetriever = generalDataRetriever;
			this.dateTimeGetter = dateTimeGetter;
			this.analyticsEventFactory = analyticsEventFactory;
			this.analytics = analytics;
		}

		public string GetVideoPath()
		{
			string url = this.adCreativeSelector.GetCreativeForOrientation(this.data.AssetVideo).Url;
			ILimitedUrlCacher limitedUrlCacher = this.adCacherRetriever.GetLimitedUrlCacher(url);
			return limitedUrlCacher.GetCachePath(url);
		}

		public Texture2D GetImage()
		{
			string url = this.adCreativeSelector.GetCreativeForOrientation(this.data.AssetImage).Url;
			ILimitedUrlCacher limitedUrlCacher = this.adCacherRetriever.GetLimitedUrlCacher(url);
			string cachePath = limitedUrlCacher.GetCachePath(url);
			return this.textureLoader.LoadTexture(cachePath, false);
		}

		public Texture2D GetButtonImage()
		{
			string buttonImageUrl = this.data.ButtonImageUrl;
			ILimitedUrlCacher limitedUrlCacher = this.adCacherRetriever.GetLimitedUrlCacher(buttonImageUrl);
			string cachePath = limitedUrlCacher.GetCachePath(buttonImageUrl);
			return this.textureLoader.LoadTexture(cachePath, false);
		}

		public CrossPromotionVideoAdCreative GetVideoCreative()
		{
			return (CrossPromotionVideoAdCreative)this.adCreativeSelector.GetCreativeForOrientation(this.data.AssetVideo);
		}

		public bool IsCached()
		{
			List<string> assetUrls = this.GetAssetUrls();
			foreach (string url in assetUrls)
			{
				ILimitedUrlCacher limitedUrlCacher = this.adCacherRetriever.GetLimitedUrlCacher(url);
				if (!limitedUrlCacher.IsCached(url))
				{
					return false;
				}
			}
			return true;
		}

		public void EnsureIsCached()
		{
			List<string> assetUrls = this.GetAssetUrls();
			foreach (string url in assetUrls)
			{
				ILimitedUrlCacher limitedUrlCacher = this.adCacherRetriever.GetLimitedUrlCacher(url);
				limitedUrlCacher.EnsureAssetIsCached(url);
			}
		}

		public void ReportAsShown(AdGroupContext adGroupContext)
		{
			object eventObject = this.analyticsEventFactory.CreateImpressionEvent(this.data, adGroupContext);
			this.analytics.LogEvent(eventObject, -1.0, null);
			this.promotedGameLauncher.LogAdjustImpression(this.data, adGroupContext, this.data);
		}

		public void ReportAsClicked(AdGroupContext adGroupContext)
		{
			object eventObject = this.analyticsEventFactory.CreateClickEvent(this.data, adGroupContext);
			this.analytics.LogEvent(eventObject, -1.0, null);
		}

		public void ReportAsCompletedWatching(AdGroupContext adGroupContext)
		{
			object eventObject = this.analyticsEventFactory.CreateCompletedEvent(this.data, adGroupContext);
			this.analytics.LogEvent(eventObject, -1.0, null);
			this.session.IncrementNumberOfTimesShown();
		}

		public void ReportAsClosed(AdGroupContext adGroupContext)
		{
			object eventObject = this.analyticsEventFactory.CreateClosedEvent(this.data, adGroupContext);
			this.analytics.LogEvent(eventObject, -1.0, null);
		}

		public bool CanShowInThisSession()
		{
			return this.session.CanShowInThisSession();
		}

		public bool HasExpired()
		{
			CrossPromotionGeneralData generalData = this.generalDataRetriever.GetGeneralData();
			int maxAdAge = generalData.CrossPromotionClientConfiguration.MaxAdAge;
			return this.dateTimeGetter.UtcNow.Subtract(this.requestTime).TotalSeconds > (double)maxAdAge;
		}

		public void OpenEmbeddedStoreIfPossible(AdGroupContext adGroupContext, Action onComplete)
		{
			onComplete();
		}

		public void SendToStoreOrLaunchGame(AdGroupContext adGroupContext, Action onComplete)
		{
			this.promotedGameLauncher.SendToStoreOrLaunchGame(this.data, adGroupContext, this.data, onComplete);
		}

		private List<string> GetAssetUrls()
		{
			List<string> list = new List<string>();
			if (this.data.AssetImage != null)
			{
				string url = this.adCreativeSelector.GetCreativeForOrientation(this.data.AssetImage).Url;
				list.Add(url);
			}
			if (this.data.AssetVideo != null)
			{
				string url2 = this.adCreativeSelector.GetCreativeForOrientation(this.data.AssetVideo).Url;
				list.Add(url2);
			}
			if (this.data.ButtonImageUrl != null)
			{
				list.Add(this.data.ButtonImageUrl);
			}
			return list;
		}

		private readonly CrossPromotionAdMetaData data;

		private readonly DateTime requestTime;

		private readonly ILimitedUrlCacherRetriever adCacherRetriever;

		private readonly IAdCreativeSelector adCreativeSelector;

		private readonly IPromotedGameLauncher promotedGameLauncher;

		private readonly ITextureLoader textureLoader;

		private readonly IAdSession session;

		private readonly IGeneralDataRetriever generalDataRetriever;

		private readonly ITactileDateTime dateTimeGetter;

		private readonly ICrossPromotionAnalyticsEventFactory analyticsEventFactory;

		private readonly IAnalytics analytics;
	}
}
