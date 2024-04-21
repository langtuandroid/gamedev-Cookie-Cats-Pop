using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelAttemptResponder
	{
		IEnumerator AttemptStarted(ILevelAttempt attempt);

		IEnumerator AttemptEnded(ILevelAttempt attempt);

		IEnumerator ContinueDismissed(ILevelAttempt attempt, EnumeratorResult<bool> keepPlaying);
	}
}
