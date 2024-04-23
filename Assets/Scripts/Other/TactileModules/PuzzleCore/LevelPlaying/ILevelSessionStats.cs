using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleCore.LevelPlaying
{
	public interface ILevelSessionStats
	{
		string LevelGuid { get; set; }

		int Score { get; }

		int Stars { get; }

		int IngameBoostersUsed { get; }

		int PresentsCollected { get; }

		bool FreebiePaid { get; }

		bool FreebieVideoWatched { get; }

		string FreebieType { get; }

		int MovesAddedByContinue { get; }

		int MovesAddedByFreebie { get; }

		int MovesAddedByGamePiece { get; }

		int MovesAddedByInGameBooster { get; }

		int MovesAddedByPreGameBooster { get; }

		int MovesLeftBeforeAftermath { get; }

		int MovesUsed { get; }

		int GoalPiecesCollected { get; }

		int Shuffles { get; }

		Dictionary<string, int> GoalAmountsLeft { get; set; }

		LevelEndState EndState { get; set; }
	}
}
