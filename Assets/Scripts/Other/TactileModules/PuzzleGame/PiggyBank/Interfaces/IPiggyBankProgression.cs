using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankProgression
	{
		int StartingLevel { get; }

		bool TutorialShown { get; }

		void SetStartingLevel(int startLevel);

		bool IsAvailableOnLevel(int humanLevel);

		bool IsNextFreeOpenReady();

		bool IsFeatureEnabled();

		int GetNextFreeOpenLevelHumanNumber();

		void UpdateToNextFreeLevel();

		void ShowedTutorial();

		bool HasHandledUnlockVisuals();

		void UnlockVisualsHandled();

		void ResetUnlockVisualsHandled();

		void SavePersistabelState();
	}
}
