using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Analytics
{
	public interface IFeatureReceivedEventLoggingStateHandler
	{
		bool CanLogReceivedEvent(FeatureData featureData);

		List<string> GetDisappearedFeatures(List<FeatureData> featureDatas);
	}
}
