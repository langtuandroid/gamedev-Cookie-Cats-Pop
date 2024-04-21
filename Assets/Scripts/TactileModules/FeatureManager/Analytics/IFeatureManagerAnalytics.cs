using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	public interface IFeatureManagerAnalytics
	{
		void LogMergeMultipleFailed(IFeatureTypeHandler featureTypeHandler, FeatureTypeData current, FeatureTypeData cloud, Exception exception);

		void LogMergeSingleFailed(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData current, ActivatedFeatureInstanceData cloud, Exception exception);

		void LogFailedUpgradingMetaData(Exception e, IFeatureTypeHandler feautreHandler, ActivatedFeatureInstanceData instanceData, int metaVersion);

		void LogCleanedInvalidFeatureState(StackTrace stackTrace, IFeatureTypeHandler featureHandler, ActivatedFeatureInstanceData instance0, ActivatedFeatureInstanceData instance1, ActivatedFeatureInstanceData merged);

		void LogEndingNullFeatureInstance(StackTrace stackTrace, IFeatureTypeHandler featureHandler, FeatureTypeData featureTypeData);

		void LogNullActivatedFeatures(StackTrace stackTrace, IFeatureTypeHandler featureHandler);

		void LogMergingMultipleActiveInstancesOfSingleFeature(StackTrace stackTrace, IFeatureTypeHandler featureHandler, string currentInstanceID, string cloudInstanceID);

		void LogFeatureActivatedEvent(FeatureData featureData);

		void LogFeatureDeactivatedEvent(FeatureData featureData);

		void LogFeatureReceivedEvent(FeatureData featureData);

		void LogFeatureAvailableEvent(FeatureData featureData);

		void LogFeatureUnAvailableEvent(FeatureData featureData, string reason);

		void LogFixedIncompatibleFeatureDataTypes(FeatureTypeData featureTypeData, string data, string expectedType);

		void LogFeatureDisappearedEvent(List<FeatureData> featureData);

		void LogClientErrorEvent(ClientErrorEvent clientErrorEvent);
	}
}
