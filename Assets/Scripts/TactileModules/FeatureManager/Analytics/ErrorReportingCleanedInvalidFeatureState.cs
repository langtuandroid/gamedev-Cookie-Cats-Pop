using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingCleanedInvalidFeatureState", false)]
	public class ErrorReportingCleanedInvalidFeatureState
	{
		public ErrorReportingCleanedInvalidFeatureState(IFeatureTypeHandler featureHandler, ActivatedFeatureInstanceData instance0, ActivatedFeatureInstanceData instance1, ActivatedFeatureInstanceData merged)
		{
			this.FeatureHandlerType = featureHandler.GetType().AssemblyQualifiedName;
			this.Instance0 = JsonSerializer.ObjectToHashtable(instance0).toJson();
			this.Instance1 = JsonSerializer.ObjectToHashtable(instance1).toJson();
			this.InstanceMerged = JsonSerializer.ObjectToHashtable(merged).toJson();
		}

		private TactileAnalytics.RequiredParam<string> FeatureHandlerType { get; set; }

		private TactileAnalytics.RequiredParam<string> Instance0 { get; set; }

		private TactileAnalytics.RequiredParam<string> Instance1 { get; set; }

		private TactileAnalytics.RequiredParam<string> InstanceMerged { get; set; }
	}
}
