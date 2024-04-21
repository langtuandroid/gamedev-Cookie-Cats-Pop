using System;
using System.Collections.Generic;
using Tactile;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;

public class SlidesAndLaddersInventory : ISlidesAndLaddersInventory
{
	public void AddToInventory(List<ItemAmount> items, string analyticsId = "")
	{
		foreach (ItemAmount itemAmount in items)
		{
			InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, (!string.IsNullOrEmpty(analyticsId)) ? analyticsId : "SlidesAndLadders");
		}
	}
}
