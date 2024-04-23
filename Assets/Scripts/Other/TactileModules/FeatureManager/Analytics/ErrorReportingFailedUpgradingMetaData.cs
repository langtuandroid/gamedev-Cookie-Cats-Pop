using System;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("errorReportingFailedUpgradingMetaData", false)]
	public class ErrorReportingFailedUpgradingMetaData
	{
		public ErrorReportingFailedUpgradingMetaData(Exception e, IFeatureTypeHandler feautreHandler, ActivatedFeatureInstanceData instanceData, int metaVersion)
		{
			this.exception = e.ToString();
			this.currentHandler = feautreHandler.GetType().AssemblyQualifiedName;
			this.currentInstanceData = JsonSerializer.ObjectToHashtable(instanceData).toJson();
			this.currentMetaVersion = metaVersion;
		}

		public TactileAnalytics.RequiredParam<string> exception { get; set; }

		public TactileAnalytics.RequiredParam<string> currentHandler { get; set; }

		public TactileAnalytics.RequiredParam<string> currentInstanceData { get; set; }

		public TactileAnalytics.RequiredParam<int> currentMetaVersion { get; set; }
	}
}
