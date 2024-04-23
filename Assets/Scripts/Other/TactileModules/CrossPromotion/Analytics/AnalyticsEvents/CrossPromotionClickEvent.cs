using System;
using TactileModules.CrossPromotion.Analytics.Data;

namespace TactileModules.CrossPromotion.Analytics.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("crossPromotionClick", true)]
	public class CrossPromotionClickEvent : CrossPromotionBasicEvent
	{
		public CrossPromotionClickEvent(CrossPromotionAnalyticsData data) : base(data)
		{
		}
	}
}
