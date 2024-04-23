using System;

namespace TactileModules.PuzzleGame.Inventory
{
	public interface IUnlimitedItems
	{
		bool HasUnlimitedTime(InventoryItem inventoryItem);

		bool IsUnlimitedType(InventoryItem inventoryItem);

		int GetTimeLeftInSeconds(InventoryItem inventoryItem);

		InventoryItem GetCorrespondingNonUnlimitedItem(InventoryItem unlimitedItem);
	}
}
