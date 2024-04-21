using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureUrlFileHandler
	{
		List<string> GetUrlsToCache(FeatureData featureData);

		void FeatureInstanceWasHidden(ActivatedFeatureInstanceData instanceData);
	}
}
