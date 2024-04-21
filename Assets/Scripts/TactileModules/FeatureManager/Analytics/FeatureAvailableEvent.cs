using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureAvailable", true)]
	public class FeatureAvailableEvent : FeatureEventBase
	{
		public FeatureAvailableEvent(FeatureData featureData) : base(featureData)
		{
		}
	}
}
