using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	public abstract class FeatureEventBase : BasicEvent
	{
		protected FeatureEventBase(FeatureData featureData)
		{
			this.FeatureType = featureData.Type;
			this.FeatureInstanceId = featureData.Id;
			object obj = featureData.MetaData["isLocal"];
			if (obj != null && (bool)obj)
			{
				this.IsLocal = true;
			}
			else
			{
				this.IsLocal = false;
			}
		}

		private TactileAnalytics.OptionalParam<string> FeatureInstanceId { get; set; }

		private TactileAnalytics.OptionalParam<string> FeatureType { get; set; }

		private TactileAnalytics.OptionalParam<bool> IsLocal { get; set; }
	}
}
