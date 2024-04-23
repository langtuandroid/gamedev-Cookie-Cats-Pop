using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.FeatureManager
{
	public interface IFeatureActivation
	{
		void ActivateFeature();

		void ActivateLocalFeature();

		void DeactivateFeature();

		bool ShouldActivateFeature();

		bool ShouldActivateLocalFeature(int unlockedLevelIndex);

		bool ShouldDeactivateFeature();

		bool HasActiveFeature();

		int GetSecondsLeft();

		event Action<ActivatedFeatureInstanceData> FeatureActivated;

		event Action<ActivatedFeatureInstanceData> FeatureDeactivated;
	}
}
