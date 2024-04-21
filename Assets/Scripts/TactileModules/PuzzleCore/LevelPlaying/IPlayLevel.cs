using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayLevel
	{
		LevelInformation GetLevelInformation();

		void Initialize(ILevelProxy levelProxy);

		void CreateViews(ILevelStartInfo levelStartInfo);

		void DestroyViews();

		LevelAttemptInfo StartAttempt();

		IEnumerator PlayUntilImmediateEndState(EnumeratorResult<LevelEndResult> outEndState);

		ILevelSessionStats ConcludeAttemptAndGetStats();

		IEnumerator RunContinueFlow(EnumeratorResult<bool> outDidContinue);

		IEnumerator ShowOutOfLivesView(EnumeratorResult<bool> outDidCancel);

		ILevelStartInfo CreateLevelStartInfo();
	}
}
