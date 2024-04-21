using System;
using TactileModules.CrossPromotion.Analytics.Data;

namespace TactileModules.CrossPromotion.Analytics.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("crossPromotionImpression", true)]
	public class CrossPromotionImpressionEvent : CrossPromotionBasicEvent
	{
		public CrossPromotionImpressionEvent(CrossPromotionAnalyticsData data) : base(data)
		{
		}
	}
}
