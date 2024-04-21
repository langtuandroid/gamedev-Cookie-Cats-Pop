using System;
using Tactile;

namespace TactileModules.GameCore.Inventory
{
	public interface IVisualInventory
	{
		int GetVisualAmount(InventoryItem item);

		event Action<InventoryItem> VisualInventoryChanged;

		IInventoryItemAnimator CreateAnimator();

		IInventoryItemAnimator CreateAnimator(IFlyingItemsAnimator customAnimator);

		IInventoryItemAnimator CreateAnimator(Func<InventoryItem, int, bool> filterFunction);

		IInventoryItemAnimator CreateAnimator(Func<InventoryItem, int, bool> filterFunction, IFlyingItemsAnimator customAnimator);

		InventoryManager InventoryManager { get; }
	}
}
