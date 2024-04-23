using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingMergeSingleFailed", false)]
	public class ErrorReportingMergeSingleFailed
	{
		public ErrorReportingMergeSingleFailed(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData current, ActivatedFeatureInstanceData cloud, Exception exception)
		{
			this.featureHandlerType = ((featureTypeHandler != null) ? featureTypeHandler.GetType().AssemblyQualifiedName : "null");
			this.currentInstanceData = ((current != null) ? JsonSerializer.ObjectToHashtable(current).toJson() : "null");
			this.cloudInstanceData = ((cloud != null) ? JsonSerializer.ObjectToHashtable(cloud).toJson() : "null");
			this.exception = exception.ToString();
		}

		private TactileAnalytics.RequiredParam<string> featureHandlerType { get; set; }

		private TactileAnalytics.RequiredParam<string> currentInstanceData { get; set; }

		private TactileAnalytics.RequiredParam<string> cloudInstanceData { get; set; }

		private TactileAnalytics.RequiredParam<string> exception { get; set; }
	}
}
