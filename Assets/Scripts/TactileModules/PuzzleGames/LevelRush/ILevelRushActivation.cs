using System;
using TactileModules.FeatureManager.DataClasses;

namespace TactileModules.PuzzleGames.LevelRush
{
	public interface ILevelRushActivation
	{
		event Action<ActivatedFeatureInstanceData> FeatureDeactivated;

		void ActivateLevelRush();

		void ActivateLocalLevelRush();

		void DeactivateLevelRush();

		bool ShouldActivateLevelRush();

		bool ShouldDeactivateLevelRush();

		bool HasPlayerAnyProgress();

		bool HasActiveFeature();

		bool FeatureEnabled();

		bool HasActivationTriggerForLevel(int levelIndex);

		int GetSecondsLeft();
	}
}
