using System;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankProvider
	{
		event Action LevelPlayed;

		event Action InGameBoosterUsed;

		void SavePersistableState();
	}
}
