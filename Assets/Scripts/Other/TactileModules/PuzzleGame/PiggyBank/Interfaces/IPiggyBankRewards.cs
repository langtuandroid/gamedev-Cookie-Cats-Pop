using System;
using System.Collections.Generic;

namespace TactileModules.PuzzleGame.PiggyBank.Interfaces
{
	public interface IPiggyBankRewards
	{
		int CollectedCoins { get; }

		int Capacity { get; }

		int CapacityIncrease { get; }

		int MaxCapacity { get; }

		int AvailableCapacityIncrease { get; }

		int FreeOpenInterval { get; }

		int CoinsRequiredForPaidOpening { get; }

		float CoinProgress { get; }

		void ResetCoins();

		void AddBoosterCoins();

		void AddPlayedCoins();

		void IncreaseCapacity();

		bool PaidOpeningReady();

		void SavePersistabelState();

		void GiveContentToPlayer();

		void GiveOfferItemsToPlayer(List<ItemAmount> items);
	}
}
