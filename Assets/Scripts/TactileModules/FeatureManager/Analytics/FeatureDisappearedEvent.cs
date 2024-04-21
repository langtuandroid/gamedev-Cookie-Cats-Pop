using System;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureDisappeared", true)]
	public class FeatureDisappearedEvent : BasicEvent
	{
		public FeatureDisappearedEvent(string featureInstanceId)
		{
			this.FeatureInstanceId = featureInstanceId;
		}

		private TactileAnalytics.RequiredParam<string> FeatureInstanceId { get; set; }
	}
}
