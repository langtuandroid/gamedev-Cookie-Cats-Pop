using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferBuyAborted", true)]
	public class SpecialOfferBuyAbortedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferBuyAbortedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
		}
	}
}
