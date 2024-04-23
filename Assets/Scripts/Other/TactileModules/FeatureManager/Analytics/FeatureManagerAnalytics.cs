using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	public class FeatureManagerAnalytics : IFeatureManagerAnalytics
	{
		public FeatureManagerAnalytics(IAnalytics analytics, IFeatureReceivedEventLoggingStateHandler featureReceivedEventLoggingStateHandler)
		{
			this.analytics = analytics;
			this.featureReceivedEventLoggingStateHandler = featureReceivedEventLoggingStateHandler;
		}

		public void LogMergeMultipleFailed(IFeatureTypeHandler featureTypeHandler, FeatureTypeData current, FeatureTypeData cloud, Exception exception)
		{
			string data = (featureTypeHandler != null) ? featureTypeHandler.GetType().AssemblyQualifiedName : "null";
			string data2 = (current != null) ? JsonSerializer.ObjectToHashtable(current).toJson() : "null";
			string data3 = (cloud != null) ? JsonSerializer.ObjectToHashtable(cloud).toJson() : "null";
			this.analytics.LogEvent(new ClientErrorEvent("MergeMultipleFailed", exception.StackTrace, exception, data, data2, data3, null, null, null), -1.0, null);
		}

		public void LogMergeSingleFailed(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData current, ActivatedFeatureInstanceData cloud, Exception exception)
		{
			string data = (featureTypeHandler != null) ? featureTypeHandler.GetType().AssemblyQualifiedName : "null";
			string data2 = (current != null) ? JsonSerializer.ObjectToHashtable(current).toJson() : "null";
			string data3 = (cloud != null) ? JsonSerializer.ObjectToHashtable(cloud).toJson() : "null";
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("MergeSingleFailed", exception.StackTrace, exception, data, data2, data3, null, null, null), -1.0, null);
		}

		public void LogFailedUpgradingMetaData(Exception e, IFeatureTypeHandler feautreHandler, ActivatedFeatureInstanceData instanceData, int metaVersion)
		{
			string assemblyQualifiedName = feautreHandler.GetType().AssemblyQualifiedName;
			string data = JsonSerializer.ObjectToHashtable(instanceData).toJson();
			int num = metaVersion;
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("FailedUpgradingMetaData", e.StackTrace, e, assemblyQualifiedName, data, num.ToString(), null, null, null), -1.0, null);
		}

		public void LogCleanedInvalidFeatureState(StackTrace stackTrace, IFeatureTypeHandler featureHandler, ActivatedFeatureInstanceData instance0, ActivatedFeatureInstanceData instance1, ActivatedFeatureInstanceData merged)
		{
			string assemblyQualifiedName = featureHandler.GetType().AssemblyQualifiedName;
			string data = JsonSerializer.ObjectToHashtable(instance0).toJson();
			string data2 = JsonSerializer.ObjectToHashtable(instance1).toJson();
			string data3 = JsonSerializer.ObjectToHashtable(merged).toJson();
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("CleanedInvalidFeatureState", stackTrace.ToString(), null, assemblyQualifiedName, data, data2, data3, null, null), -1.0, null);
		}

		public void LogFixedIncompatibleFeatureDataTypes(FeatureTypeData featureTypeData, string data, string expectedType)
		{
			this.analytics.LogEvent(new ErrorReportingFixedIncompatibleFeatureDataTypes(featureTypeData, data, expectedType), -1.0, null);
		}

		public void LogEndingNullFeatureInstance(StackTrace stackTrace, IFeatureTypeHandler featureHandler, FeatureTypeData featureTypeData)
		{
			string assemblyQualifiedName = featureHandler.GetType().AssemblyQualifiedName;
			string data = JsonSerializer.ObjectToHashtable(featureTypeData).toJson();
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("EndingNullFeatureInstance", stackTrace.ToString(), null, assemblyQualifiedName, data, null, null, null, null), -1.0, null);
		}

		public void LogNullActivatedFeatures(StackTrace stackTrace, IFeatureTypeHandler featureHandler)
		{
			string featureType = featureHandler.FeatureType;
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("NullActivatedFeatures", stackTrace.ToString(), null, featureType, null, null, null, null, null), -1.0, null);
		}

		public void LogMergingMultipleActiveInstancesOfSingleFeature(StackTrace stackTrace, IFeatureTypeHandler featureHandler, string currentInstanceID, string cloudInstanceID)
		{
			string featureType = featureHandler.FeatureType;
			TactileAnalytics.Instance.LogEvent(new ClientErrorEvent("MergingMultipleActiveInstancesOfSingleFeature", stackTrace.ToString(), null, featureType, currentInstanceID, cloudInstanceID, null, null, null), -1.0, null);
		}

		public void LogFeatureActivatedEvent(FeatureData featureData)
		{
			this.analytics.LogEvent(new FeatureActivated(featureData), -1.0, null);
		}

		public void LogFeatureDeactivatedEvent(FeatureData featureData)
		{
			this.analytics.LogEvent(new FeatureDeactivated(featureData), -1.0, null);
		}

		public void LogFeatureReceivedEvent(FeatureData featureData)
		{
			if (this.featureReceivedEventLoggingStateHandler.CanLogReceivedEvent(featureData))
			{
				this.analytics.LogEvent(new FeatureReceivedEvent(featureData), -1.0, null);
			}
		}

		public void LogFeatureDisappearedEvent(List<FeatureData> featureData)
		{
			List<string> disappearedFeatures = this.featureReceivedEventLoggingStateHandler.GetDisappearedFeatures(featureData);
			foreach (string featureInstanceId in disappearedFeatures)
			{
				this.analytics.LogEvent(new FeatureDisappearedEvent(featureInstanceId), -1.0, null);
			}
		}

		public void LogClientErrorEvent(ClientErrorEvent clientErrorEvent)
		{
			this.analytics.LogEvent(clientErrorEvent, -1.0, null);
		}

		public void LogFeatureAvailableEvent(FeatureData featureData)
		{
			this.analytics.LogEvent(new FeatureAvailableEvent(featureData), -1.0, null);
		}

		public void LogFeatureUnAvailableEvent(FeatureData featureData, string reason)
		{
			this.analytics.LogEvent(new FeatureUnAvailableEvent(featureData, reason), -1.0, null);
		}

		private readonly IAnalytics analytics;

		private readonly IFeatureReceivedEventLoggingStateHandler featureReceivedEventLoggingStateHandler;

		private FeatureReceivedEventLoggingState featureReceivedEventLoggingState;
	}
}
