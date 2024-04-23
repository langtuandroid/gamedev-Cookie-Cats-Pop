using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingFixedIncompatibleFeatureDataTypes", true)]
	public class ErrorReportingFixedIncompatibleFeatureDataTypes
	{
		public ErrorReportingFixedIncompatibleFeatureDataTypes(FeatureTypeData featureTypeData, string currentData, string expectedData)
		{
			this.FeatureTypeData = JsonSerializer.ObjectToHashtable(featureTypeData).toJson();
			this.CurrentData = currentData;
			this.ExpectedData = expectedData;
		}

		private TactileAnalytics.RequiredParam<string> FeatureTypeData { get; set; }

		private TactileAnalytics.RequiredParam<string> CurrentData { get; set; }

		private TactileAnalytics.RequiredParam<string> ExpectedData { get; set; }
	}
}
