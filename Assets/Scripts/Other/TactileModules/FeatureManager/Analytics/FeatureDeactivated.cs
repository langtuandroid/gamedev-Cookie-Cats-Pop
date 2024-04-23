using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureEnded", true)]
	public class FeatureDeactivated : FeatureEventBase
	{
		public FeatureDeactivated(FeatureData featureData) : base(featureData)
		{
		}
	}
}
