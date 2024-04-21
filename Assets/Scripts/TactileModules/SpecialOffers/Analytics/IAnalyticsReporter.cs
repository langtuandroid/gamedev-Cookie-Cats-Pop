using System;
using System.Collections.Generic;
using TactileModules.SpecialOffers.Model;

namespace TactileModules.SpecialOffers.Analytics
{
	public interface IAnalyticsReporter
	{
		void LogSpecialOfferSelectorNoOfferAvailable(List<ISpecialOffer> allOffers, string noOfferAvailableReason);

		void LogSpecialOfferFlowStarted(string featureInstanceId, FlowStartedReason flowStartedReason);

		void LogSpecialOfferActivated(string featureInstanceId);

		void LogSpecialOfferDeactivated(string featureInstanceId, DeactivationReason deactivationReason);

		void LogSpecialOfferBuyStarted(string featureInstanceId);

		void LogSpecialOfferBuyPurchased(string featureInstanceId);

		void LogSpecialOfferBuyAborted(string featureInstanceId);
	}
}
