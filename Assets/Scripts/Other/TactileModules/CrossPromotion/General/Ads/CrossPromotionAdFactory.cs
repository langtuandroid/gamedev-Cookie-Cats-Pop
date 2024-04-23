using System;
using TactileModules.Analytics.Interfaces;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataRetrievers;
using TactileModules.CrossPromotion.General.Ads.AdModels;
using TactileModules.CrossPromotion.General.LimitedUrlCaching;
using TactileModules.CrossPromotion.General.PromotedGameUtility;
using TactileModules.RuntimeTools;

namespace TactileModules.CrossPromotion.General.Ads
{
	public class CrossPromotionAdFactory : ICrossPromotionAdFactory
	{
		public CrossPromotionAdFactory(ILimitedUrlCacherRetriever adCacherRetriever, IAdCreativeSelector adCreativeSelector, IPromotedGameLauncher promotedGameLauncher, IGeneralDataRetriever generalDataRetriever, ITactileDateTime dateTimeGetter, ITextureLoader textureLoader, IAnalytics analytics)
		{
			this.adCacherRetriever = adCacherRetriever;
			this.adCreativeSelector = adCreativeSelector;
			this.promotedGameLauncher = promotedGameLauncher;
			this.generalDataRetriever = generalDataRetriever;
			this.dateTimeGetter = dateTimeGetter;
			this.textureLoader = textureLoader;
			this.analytics = analytics;
		}

		public ICrossPromotionAd Create(CrossPromotionAdMetaData metaData, DateTime requestTime, IAdSession session, ICrossPromotionAnalyticsEventFactory analyticsEventFactory)
		{
			return new CrossPromotionAd(metaData, requestTime, this.adCacherRetriever, this.adCreativeSelector, this.promotedGameLauncher, this.textureLoader, session, this.generalDataRetriever, this.dateTimeGetter, analyticsEventFactory, this.analytics);
		}

		private readonly ILimitedUrlCacherRetriever adCacherRetriever;

		private readonly IAdCreativeSelector adCreativeSelector;

		private readonly ITextureLoader textureLoader;

		private readonly IAnalytics analytics;

		private readonly IPromotedGameLauncher promotedGameLauncher;

		private readonly IGeneralDataRetriever generalDataRetriever;

		private readonly ITactileDateTime dateTimeGetter;
	}
}
