using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ICorePlayFlow : IFiberRunnable, IFullScreenOwner, ILevelAttemptResponder
	{
		event Action<ILevelSessionRunner> LevelSessionStarted;

		event Action<ILevelSessionRunner> LevelSessionEnded;

		IHookList<LevelProxy, bool> StartViewInitializedHook { get; }

		IHookList<ILevelAttempt> LevelStartedHook { get; }

		IBreakableHookList<ILevelAttempt> ContinueDismissedHook { get; }

		IHookList<ILevelAttempt> LevelEndedHook { get; }

		IHookList<ICorePlayFlow, PostLevelPlayedAction> ResultsShownHook { get; }

		IHookList<ILevelSessionRunner> UserDismissedOutOfLives { get; }

		LevelProxy LevelProxy { get; }

		PostLevelPlayedAction FinalPostLevelAction { get; }

		ILevelAttempt FinalLevelAttempt { get; }

		bool CancelledAtFirstBoosterSelection { get; }

		IPlayLevel GameImplementation { get; }

		IPlayFlowContext PlayFlowContext { get; }

		IEnumerator EnsureGameScreen();
	}
}
