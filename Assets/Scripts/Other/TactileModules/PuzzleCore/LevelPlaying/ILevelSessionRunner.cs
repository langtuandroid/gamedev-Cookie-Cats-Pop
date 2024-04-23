using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelSessionRunner
	{
		string SessionId { get; }

		LevelProxy LevelProxy { get; }

		LevelInformation LevelInformation { get; }

		ILevelStartInfo LevelStartInfo { get; }

		PostLevelPlayedAction EndAction { get; }

		IPlayLevel GameImplementation { get; }

		ICorePlayFlow PlayFlow { get; }

		IEnumerator Run(int priorAttempts, EnumeratorResult<ILevelAttempt> attempt, EnumeratorResult<bool> playLevelAgain);
	}
}
