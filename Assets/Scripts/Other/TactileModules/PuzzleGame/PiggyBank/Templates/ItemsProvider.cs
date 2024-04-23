using System;
using System.Collections.Generic;
using Shared.PiggyBank.Module.Interfaces;
using Tactile;

namespace TactileModules.PuzzleGame.PiggyBank.Templates
{
	public class ItemsProvider : IItemsProvider
	{
		public void GiveContentToPlayer(int content)
		{
			InventoryManager.Instance.Add("Coin", content, "PiggyBank");
		}

		public void GiveOfferItemsToPlayer(List<ItemAmount> items)
		{
			foreach (ItemAmount itemAmount in items)
			{
				InventoryManager.Instance.Add(itemAmount.ItemId, itemAmount.Amount, "PiggyBankOffer");
			}
		}
	}
}
