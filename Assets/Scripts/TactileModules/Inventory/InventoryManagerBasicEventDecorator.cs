using System;
using Tactile;
using TactileModules.Analytics.Interfaces;

namespace TactileModules.Inventory
{
	public class InventoryManagerBasicEventDecorator : IEventDecorator<BasicEvent>, IEventDecorator
	{
		public InventoryManagerBasicEventDecorator(InventoryManager inventoryManagerInstance)
		{
			this.inventoryManager = inventoryManagerInstance;
		}

		public void Decorate(BasicEvent basicEvent)
		{
			int lives = this.inventoryManager.Lives;
			basicEvent.SetInventoryManagerParameters(lives);
		}

		private readonly InventoryManager inventoryManager;
	}
}
