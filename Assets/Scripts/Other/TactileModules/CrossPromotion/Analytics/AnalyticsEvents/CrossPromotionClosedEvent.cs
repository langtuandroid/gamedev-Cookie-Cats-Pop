using System;
using TactileModules.CrossPromotion.Analytics.Data;

namespace TactileModules.CrossPromotion.Analytics.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("crossPromotionClosed", true)]
	public class CrossPromotionClosedEvent : CrossPromotionBasicEvent
	{
		public CrossPromotionClosedEvent(CrossPromotionAnalyticsData data) : base(data)
		{
		}
	}
}
