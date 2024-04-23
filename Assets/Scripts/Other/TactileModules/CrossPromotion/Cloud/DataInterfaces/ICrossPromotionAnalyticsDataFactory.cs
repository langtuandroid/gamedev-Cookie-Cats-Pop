using System;
using TactileModules.Ads.Analytics;
using TactileModules.CrossPromotion.Analytics.Data;
using TactileModules.CrossPromotion.Cloud.Data;
using TactileModules.CrossPromotion.General;

namespace TactileModules.CrossPromotion.Cloud.DataInterfaces
{
	public interface ICrossPromotionAnalyticsDataFactory
	{
		CrossPromotionAnalyticsData CreateData(AdType type, CrossPromotionAdMetaData data, AdGroupContext adGroupContext);
	}
}
