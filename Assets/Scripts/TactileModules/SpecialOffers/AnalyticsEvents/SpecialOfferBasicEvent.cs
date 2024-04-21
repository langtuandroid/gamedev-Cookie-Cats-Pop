using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.AnalyticsEvents
{
	public class SpecialOfferBasicEvent : SpecialOffersBasicEvent
	{
		public SpecialOfferBasicEvent(ISpecialOffersGlobalCoolDown specialOffersGlobalCoolDown, FeatureData featureData, SpecialOfferMetaData metaData) : base(specialOffersGlobalCoolDown)
		{
			this.FeatureInstanceId = featureData.Id;
			this.AnalyticsId = metaData.AnalyticsId;
			this.RequiredLevel = metaData.RequiredLevel;
			this.SpecialOfferType = metaData.SpecialOfferType.ToString();
			this.IAPIdentifier = ((!string.IsNullOrEmpty(metaData.IAPIdentifier)) ? metaData.IAPIdentifier : null);
		}

		private TactileAnalytics.RequiredParam<string> FeatureInstanceId { get; set; }

		private TactileAnalytics.RequiredParam<string> AnalyticsId { get; set; }

		private TactileAnalytics.RequiredParam<int> RequiredLevel { get; set; }

		private TactileAnalytics.RequiredParam<string> SpecialOfferType { get; set; }

		private TactileAnalytics.OptionalParam<string> IAPIdentifier { get; set; }
	}
}
