using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferDeactivated", true)]
	public class SpecialOfferDeactivatedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferDeactivatedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData, DeactivationReason deactivationReason) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
			this.DeactivationReason = deactivationReason.ToString();
		}

		private TactileAnalytics.RequiredParam<string> DeactivationReason { get; set; }
	}
}
