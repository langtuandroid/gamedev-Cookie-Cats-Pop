using System;

namespace TactileModules.GameCore.Inventory
{
	public class InventorySystem
	{
		public InventorySystem(VisualInventory visualInventory)
		{
			this.VisualInventory = visualInventory;
		}

		public VisualInventory VisualInventory { get; private set; }
	}
}
