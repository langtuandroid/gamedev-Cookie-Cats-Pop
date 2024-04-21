using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	internal interface IMainLevelVictory : IGameInterface
	{
		IEnumerator ShowVictory(ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> action);
	}
}
