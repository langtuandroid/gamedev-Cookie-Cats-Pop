using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureAssetBundleHandler
	{
		List<string> GetAssetBundles(FeatureData featureData);

		void FeatureInstanceWasHidden(ActivatedFeatureInstanceData instanceData);
	}
}
