using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager.Interfaces
{
	public interface IEndPreviousFeature
	{
		void EndPreviousInstanceOfFeature(ActivatedFeatureInstanceData activatedFeatureInstanceData);
	}
}
