using System;
using TactileModules.CrossPromotion.Analytics;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.General.Ads.AdModels;

namespace TactileModules.CrossPromotion.General.Ads
{
	public interface ICrossPromotionAdFactory
	{
		ICrossPromotionAd Create(CrossPromotionAdMetaData metaData, DateTime requestTime, IAdSession session, ICrossPromotionAnalyticsEventFactory analyticsEventFactory);
	}
}
