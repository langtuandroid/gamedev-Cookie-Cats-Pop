using System;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	public class SpecialOffersBasicEvent : BasicEvent
	{
		public SpecialOffersBasicEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown)
		{
			this.GlobalCooldownTimestamp = DateHelper.GetDateTimeFromUnixTimestamp((long)specialOffersGlobalCoolDown.GetCoolDownTimeStamp());
		}

		private TactileAnalytics.RequiredParam<DateTime> GlobalCooldownTimestamp { get; set; }
	}
}
