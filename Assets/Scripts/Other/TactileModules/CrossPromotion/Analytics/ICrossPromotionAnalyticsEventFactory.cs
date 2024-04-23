using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.General.Ads.AdModels;

namespace TactileModules.CrossPromotion.Analytics
{
	public interface ICrossPromotionAnalyticsEventFactory
	{
		object CreateImpressionEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext);

		object CreateClickEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext);

		object CreateCompletedEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext);

		object CreateClosedEvent(CrossPromotionAdMetaData data, AdGroupContext adGroupContext);

		object CreateCrossPromotionRewardedVideoFreezeEvent(ICrossPromotionAd crossPromotionAd, int videoFrame);
	}
}
