using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureEndedFromMerge", true)]
	public class FeatureDeactivatedFromMerge : FeatureDeactivated
	{
		public FeatureDeactivatedFromMerge(FeatureData featureData) : base(featureData)
		{
		}
	}
}
