using System;
using System.Collections.Generic;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;
using TactileModules.SpecialOffers.AnalyticsEvents;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Analytics
{
	public class AnalyticsReporter : IAnalyticsReporter
	{
		public AnalyticsReporter(IAnalytics analytics, IFeatureManager featureManager, IFeatureTypeHandler specialOffersHandler, ISpecialOffersGlobalCoolDown globalCoolDown)
		{
			this.analytics = analytics;
			this.featureManager = featureManager;
			this.specialOffersHandler = specialOffersHandler;
			this.globalCoolDown = globalCoolDown;
			featureManager.OnFeatureListUpdated += delegate()
			{
				this.featureListUpdated = true;
			};
		}

		public void LogSpecialOfferSelectorNoOfferAvailable(List<ISpecialOffer> allOffers, string noOfferAvailableReason)
		{
			if (!this.featureListUpdated)
			{
				return;
			}
			SpecialOfferSelectorNoOfferAvailableEvent specialOfferSelectorNoOfferAvailableEvent = new SpecialOfferSelectorNoOfferAvailableEvent(this.globalCoolDown, allOffers, noOfferAvailableReason, this.featureManager.ServerTime);
			string persistedNoOfferAvailableEvent = AnalyticsReporter.PersistedNoOfferAvailableEvent;
			string allParametersCombinedIntoOneString = specialOfferSelectorNoOfferAvailableEvent.GetAllParametersCombinedIntoOneString();
			if (persistedNoOfferAvailableEvent.Equals(allParametersCombinedIntoOneString))
			{
				return;
			}
			AnalyticsReporter.PersistedNoOfferAvailableEvent = allParametersCombinedIntoOneString;
			this.analytics.LogEvent(specialOfferSelectorNoOfferAvailableEvent, -1.0, null);
		}

		public void LogSpecialOfferFlowStarted(string featureInstanceId, FlowStartedReason flowStartedReason)
		{
			this.analytics.LogEvent(new SpecialOfferFlowStartedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId), flowStartedReason), -1.0, null);
		}

		public void LogSpecialOfferActivated(string featureInstanceId)
		{
			this.analytics.LogEvent(new SpecialOfferActivatedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId)), -1.0, null);
		}

		public void LogSpecialOfferDeactivated(string featureInstanceId, DeactivationReason deactivationReason)
		{
			this.analytics.LogEvent(new SpecialOfferDeactivatedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId), deactivationReason), -1.0, null);
		}

		public void LogSpecialOfferBuyStarted(string featureInstanceId)
		{
			this.analytics.LogEvent(new SpecialOfferBuyStartedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId)), -1.0, null);
		}

		public void LogSpecialOfferBuyPurchased(string featureInstanceId)
		{
			this.analytics.LogEvent(new SpecialOfferBuyPurchasedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId)), -1.0, null);
		}

		public void LogSpecialOfferBuyAborted(string featureInstanceId)
		{
			this.analytics.LogEvent(new SpecialOfferBuyAbortedEvent(this.globalCoolDown, this.GetFeatureData(featureInstanceId), this.GetMetaData(featureInstanceId)), -1.0, null);
		}

		private FeatureData GetFeatureData(string featureInstanceId)
		{
			ActivatedFeatureInstanceData activatedInstanceData = this.GetActivatedInstanceData(featureInstanceId);
			if (activatedInstanceData != null)
			{
				return activatedInstanceData.FeatureData;
			}
			return this.featureManager.GetFeature(this.specialOffersHandler, featureInstanceId);
		}

		private SpecialOfferMetaData GetMetaData(string featureInstanceId)
		{
			FeatureData featureData = this.GetFeatureData(featureInstanceId);
			if (featureData != null)
			{
				return this.featureManager.GetFeatureInstanceMetaData<SpecialOfferMetaData>(featureData);
			}
			return null;
		}

		private ActivatedFeatureInstanceData GetActivatedInstanceData(string featureInstanceId)
		{
			return this.featureManager.GetActivatedFeature(this.specialOffersHandler, featureInstanceId);
		}

		private static string PersistedNoOfferAvailableEvent
		{
			get
			{
				return TactilePlayerPrefs.GetSecuredString("lastNoOfferAvailableEvent", string.Empty);
			}
			set
			{
				TactilePlayerPrefs.SetSecuredString("lastNoOfferAvailableEvent", value);
			}
		}

		private const string PREFS_LAST_NO_OFFER_AVAILABLE_EVENT = "lastNoOfferAvailableEvent";

		private readonly IAnalytics analytics;

		private readonly IFeatureManager featureManager;

		private readonly IFeatureTypeHandler specialOffersHandler;

		private readonly ISpecialOffersGlobalCoolDown globalCoolDown;

		private bool featureListUpdated;
	}
}
