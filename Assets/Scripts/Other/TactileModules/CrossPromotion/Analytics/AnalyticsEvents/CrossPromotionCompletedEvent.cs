using System;
using TactileModules.CrossPromotion.Analytics.Data;

namespace TactileModules.CrossPromotion.Analytics.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("crossPromotionCompleted", true)]
	public class CrossPromotionCompletedEvent : CrossPromotionBasicEvent
	{
		public CrossPromotionCompletedEvent(CrossPromotionAnalyticsData data) : base(data)
		{
		}
	}
}
