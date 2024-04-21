using System;
using Tactile;

namespace TactileModules.Inventory
{
	public interface IInventorySystem
	{
		InventoryManager InventoryManager { get; }

		InventoryEventLogger InventoryEventLogger { get; }
	}
}
