using System;
using Tactile;
using TactileModules.PuzzleGame.ScheduledBooster.Model;

public class ScheduledBoosterInventoryProvider : IScheduledBoosterInventoryProvider
{
	public int GetCoins()
	{
		return InventoryManager.Instance.Coins;
	}

	public void ConsumeCoins(int coinsAmount, string analyticsString)
	{
		InventoryManager.Instance.Consume("Coin", coinsAmount, analyticsString);
	}

	public void ReserveCoins(int boosterPrice)
	{
		InventoryManager.Instance.Reserve("Coin", boosterPrice);
	}

	public void UnreserveCoins(int boosterPrice)
	{
		InventoryManager.Instance.Unreserve("Coin", boosterPrice);
	}
}
