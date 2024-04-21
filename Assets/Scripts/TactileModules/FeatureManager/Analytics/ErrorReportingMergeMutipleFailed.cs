using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingMergeMutipleFailed", false)]
	public class ErrorReportingMergeMutipleFailed
	{
		public ErrorReportingMergeMutipleFailed(IFeatureTypeHandler featureTypeHandler, FeatureTypeData current, FeatureTypeData cloud, Exception exception)
		{
			this.featureHandlerType = ((featureTypeHandler != null) ? featureTypeHandler.GetType().AssemblyQualifiedName : "null");
			this.currentFeatureData = ((current != null) ? JsonSerializer.ObjectToHashtable(current).toJson() : "null");
			this.cloudFeatureData = ((cloud != null) ? JsonSerializer.ObjectToHashtable(cloud).toJson() : "null");
			this.exception = exception.ToString();
		}

		private TactileAnalytics.RequiredParam<string> featureHandlerType { get; set; }

		private TactileAnalytics.RequiredParam<string> currentFeatureData { get; set; }

		private TactileAnalytics.RequiredParam<string> cloudFeatureData { get; set; }

		private TactileAnalytics.RequiredParam<string> exception { get; set; }
	}
}
