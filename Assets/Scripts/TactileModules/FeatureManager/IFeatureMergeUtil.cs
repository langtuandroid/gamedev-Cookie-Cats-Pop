using System;
using System.Collections.Generic;
using TactileModules.FeatureManager.DataClasses;
using TactileModules.FeatureManager.Interfaces;

namespace TactileModules.FeatureManager
{
	public interface IFeatureMergeUtil
	{
		List<ActivatedFeatureInstanceData> MergeMultiple(IFeatureTypeHandler featureTypeHandler, FeatureTypeData current, FeatureTypeData cloud);

		ActivatedFeatureInstanceData MergeSingle(IFeatureTypeHandler featureTypeHandler, ActivatedFeatureInstanceData current, ActivatedFeatureInstanceData cloud);
	}
}
