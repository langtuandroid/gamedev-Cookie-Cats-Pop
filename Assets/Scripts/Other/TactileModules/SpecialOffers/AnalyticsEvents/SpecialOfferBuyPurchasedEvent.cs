using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferBuyPurchased", true)]
	public class SpecialOfferBuyPurchasedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferBuyPurchasedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
		}
	}
}
