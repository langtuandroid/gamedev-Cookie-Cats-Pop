using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelAttempt : IFiberRunnable
	{
		bool Completed { get; }

		ILevelSessionRunner LevelSession { get; }

		double SecondsPlayed { get; }

		ILevelSessionStats Stats { get; }

		int NumberOfContinuesUsed { get; }

		LevelEndState FinalEndState { get; }

		LevelProxy LevelProxy { get; }

		int LevelSeed { get; }

		bool WasCompletedBefore { get; }

		bool WasCompletedForTheFirstTime { get; }

		bool DidPlayAndFail { get; }

		IPlayLevel GameImplementation { get; }

		IEnumerator Run();

		void OnExit();
	}
}
