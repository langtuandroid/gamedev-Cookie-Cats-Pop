using System;
using Tactile;
using TactileModules.PuzzleGame.Inventory;

namespace TactileModules.Inventory
{
	public class InventorySystem : IInventorySystem
	{
		public InventorySystem(InventoryManager manager, InventoryEventLogger eventLogger, IUnlimitedItems unlimitedItems)
		{
			this.InventoryManager = manager;
			this.InventoryEventLogger = eventLogger;
			this.UnlimitedItems = unlimitedItems;
		}

		public InventoryManager InventoryManager { get; private set; }

		public IUnlimitedItems UnlimitedItems { get; private set; }

		public InventoryEventLogger InventoryEventLogger { get; private set; }
	}
}
