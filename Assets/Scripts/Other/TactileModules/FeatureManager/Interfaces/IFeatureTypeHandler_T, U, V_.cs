using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IFeatureTypeHandler<T, U, V> : IFeatureTypeHandler where T : FeatureInstanceCustomData where U : FeatureMetaData where V : FeatureTypeCustomData
	{
		T NewFeatureInstanceCustomData(FeatureData featureData);

		V NewFeatureTypeCustomData();

		void MergeFeatureInstanceStates<FeatureState>(ref FeatureState toMerge, FeatureState current, FeatureState cloud) where FeatureState : T;

		void MergeFeatureTypeState<HandlerState>(ref HandlerState toMerge, HandlerState current, HandlerState cloud) where HandlerState : V;
	}
}
