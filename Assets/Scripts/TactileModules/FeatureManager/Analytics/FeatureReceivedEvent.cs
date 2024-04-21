using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	[TactileAnalytics.EventAttribute("featureReceived", true)]
	public class FeatureReceivedEvent : FeatureEventBase
	{
		public FeatureReceivedEvent(FeatureData featureData) : base(featureData)
		{
			DateTime dateTime = new DateTime(featureData.StartUnixTime);
			this.StartDate = dateTime.ToString();
		}

		private TactileAnalytics.OptionalParam<string> StartDate { get; set; }
	}
}
