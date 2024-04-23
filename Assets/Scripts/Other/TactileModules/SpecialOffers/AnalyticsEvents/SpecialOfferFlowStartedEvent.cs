using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Analytics;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	[TactileAnalytics.EventAttribute("specialOfferFlowStarted", true)]
	public class SpecialOfferFlowStartedEvent : SpecialOfferBasicEvent
	{
		public SpecialOfferFlowStartedEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData, FlowStartedReason flowStartedReason) : base(specialOffersGlobalCoolDown, featureData, metaData)
		{
			this.FlowStartedReason = flowStartedReason.ToString();
		}

		private TactileAnalytics.RequiredParam<string> FlowStartedReason { get; set; }
	}
}
