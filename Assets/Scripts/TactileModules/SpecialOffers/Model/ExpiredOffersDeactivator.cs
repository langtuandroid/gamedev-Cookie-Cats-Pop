using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.SpecialOffers.Analytics;

namespace TactileModules.SpecialOffers.Model
{
	public class ExpiredOffersDeactivator : IExpiredOffersDeactivator
	{
		public ExpiredOffersDeactivator(IFeatureManager featureManager, IFeatureTypeHandler handler, IAnalyticsReporter analyticsReporter)
		{
			this.featureManager = featureManager;
			this.handler = handler;
			this.analyticsReporter = analyticsReporter;
		}

		public void DeactivateExpiredOffers()
		{
			List<ActivatedFeatureInstanceData> activatedFeatures = this.featureManager.GetActivatedFeatures(this.handler);
			foreach (ActivatedFeatureInstanceData activatedFeatureInstanceData in activatedFeatures)
			{
				if (this.featureManager.ShouldDeactivateFeature(this.handler, activatedFeatureInstanceData))
				{
					this.analyticsReporter.LogSpecialOfferDeactivated(activatedFeatureInstanceData.Id, DeactivationReason.Expiration);
					this.featureManager.DeactivateFeature(this.handler, activatedFeatureInstanceData);
				}
			}
		}

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler handler;

		private readonly IAnalyticsReporter analyticsReporter;
	}
}
