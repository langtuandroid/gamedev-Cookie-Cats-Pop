using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.SagaCore
{
	internal interface IGateVictory : IGameInterface
	{
		IEnumerator ShowVictory(GateManager gateManager, ILevelAttempt levelAttempt, EnumeratorResult<PostLevelPlayedAction> action);
	}
}
