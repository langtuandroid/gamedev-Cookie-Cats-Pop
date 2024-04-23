using System;
using System.Diagnostics;
using JetBrains.Annotations;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingEndingNullFeature", false)]
	public class ErrorReportingEndingNullFeatureInstance
	{
		public ErrorReportingEndingNullFeatureInstance(StackTrace stackTrace, IFeatureTypeHandler featureTypeHandler, FeatureTypeData featureTypeData)
		{
			this.FeatureHandlerType = featureTypeHandler.GetType().FullName;
			this.FeatureTypeData = JsonSerializer.ObjectToHashtable(featureTypeData).toJson();
			this.StackTrace = stackTrace.ToString();
		}

		private TactileAnalytics.RequiredParam<string> FeatureHandlerType { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> FeatureTypeData { [UsedImplicitly] get; set; }

		private TactileAnalytics.RequiredParam<string> StackTrace { [UsedImplicitly] get; set; }
	}
}
