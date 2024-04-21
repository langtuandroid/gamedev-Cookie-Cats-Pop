using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureStarted", true)]
	public class FeatureActivated : FeatureEventBase
	{
		public FeatureActivated(FeatureData featureData) : base(featureData)
		{
		}
	}
}
