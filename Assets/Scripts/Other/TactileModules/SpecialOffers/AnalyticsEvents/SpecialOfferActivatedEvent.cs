using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferActivated", true)]
	public class SpecialOfferActivatedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferActivatedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
		}
	}
}
