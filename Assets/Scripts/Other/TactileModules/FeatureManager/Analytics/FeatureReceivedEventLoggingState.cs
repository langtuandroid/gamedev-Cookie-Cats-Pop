using System;
using System.Collections.Generic;

namespace TactileModules.FeatureManager.Analytics
{
	public class FeatureReceivedEventLoggingState
	{
		public FeatureReceivedEventLoggingState()
		{
			this.LoggedFeatures = new List<string>();
		}

		[JsonSerializable("LoggedFeatures", typeof(string))]
		public List<string> LoggedFeatures { get; set; }

		public const int MAX_ENTRIES = 100;
	}
}
