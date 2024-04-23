using System;

namespace TactileModules.PuzzleGame.ScheduledBooster.Model
{
	public interface IScheduledBoosterInventoryProvider
	{
		int GetCoins();

		void ConsumeCoins(int coinsAmount, string analyticsString);

		void ReserveCoins(int boosterPrice);

		void UnreserveCoins(int boosterPrice);
	}
}
