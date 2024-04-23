using System;

namespace TactileModules.PuzzleGame.Inventory
{
	public interface IUnlimitedItemsProvider
	{
		bool IsUnlimitedType(InventoryItem inventoryItem);

		InventoryItem GetCorrespondingNonUnlimitedItem(InventoryItem unlimitedItem);
	}
}
