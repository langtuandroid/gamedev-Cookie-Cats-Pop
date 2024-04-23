using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferBuyStarted", true)]
	public class SpecialOfferBuyStartedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferBuyStartedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
		}
	}
}
