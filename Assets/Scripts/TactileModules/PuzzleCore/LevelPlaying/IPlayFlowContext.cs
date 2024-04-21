using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface IPlayFlowContext
	{
		bool ConsumesNormalLives { get; }

		bool AllowRetries { get; }

		bool SkipLevelStartView { get; }

		IEnumerator ShowVictoryScreenAndChooseAction(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> chosenAction);

		string GetLevelDescriptionForEndUser();
	}
}
