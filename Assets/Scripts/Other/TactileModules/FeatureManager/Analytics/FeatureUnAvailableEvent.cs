using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureUnavailable", true)]
	public class FeatureUnAvailableEvent : FeatureEventBase
	{
		public FeatureUnAvailableEvent(FeatureData featureData, string reason) : base(featureData)
		{
			this.Reason = reason;
		}

		private TactileAnalytics.RequiredParam<string> Reason { get; set; }
	}
}
