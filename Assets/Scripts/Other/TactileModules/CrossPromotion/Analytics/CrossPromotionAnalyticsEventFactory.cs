using System;
using System.Diagnostics;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Analytics.AnalyticsEvents;
using TactileModules.CrossPromotion.Analytics.Data;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.Cloud.DataInterfaces;
using TactileModules.CrossPromotion.General;
using TactileModules.CrossPromotion.General.Ads.AdModels;

namespace TactileModules.CrossPromotion.Analytics
{
	public class CrossPromotionAnalyticsEventFactory : ICrossPromotionAnalyticsEventFactory
	{
		public CrossPromotionAnalyticsEventFactory(AdType type, ICrossPromotionAnalyticsDataFactory analyticsDataFactory)
		{
			this.type = type;
			this.analyticsDataFactory = analyticsDataFactory;
		}

		public object CreateImpressionEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext)
		{
			CrossPromotionAnalyticsData data2 = this.analyticsDataFactory.CreateData(this.type, data, adGroupContext);
			return new CrossPromotionImpressionEvent(data2);
		}

		public object CreateClickEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext)
		{
			CrossPromotionAnalyticsData data2 = this.analyticsDataFactory.CreateData(this.type, data, adGroupContext);
			return new CrossPromotionClickEvent(data2);
		}

		public object CreateCompletedEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext)
		{
			CrossPromotionAnalyticsData data2 = this.analyticsDataFactory.CreateData(this.type, data, adGroupContext);
			return new CrossPromotionCompletedEvent(data2);
		}

		public object CreateClosedEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext)
		{
			CrossPromotionAnalyticsData data2 = this.analyticsDataFactory.CreateData(this.type, data, adGroupContext);
			return new CrossPromotionClosedEvent(data2);
		}

		public object CreateCrossPromotionRewardedVideoFreezeEvent(ICrossPromotionAd crossPromotionAd, int videoFrame)
		{
			return new ClientErrorEvent("CrossPromotionRewardedVideoFreeze", new StackTrace(false).ToString(), null, crossPromotionAd.GetVideoCreative().Id, videoFrame.ToString(), null, null, null, null);
		}

		private readonly AdType type;

		private readonly ICrossPromotionAnalyticsDataFactory analyticsDataFactory;
	}
}
