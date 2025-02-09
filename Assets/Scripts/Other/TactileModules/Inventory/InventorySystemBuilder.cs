using System;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGame.Inventory;
using TactileModules.Timing.Interfaces;

namespace TactileModules.Inventory
{
	public static class InventorySystemBuilder
	{
		public static IInventorySystem Build(IAdjustInventoryTracking adjustInventoryTracking, ITimingManager timingManager, IUnlimitedItemsProvider unlimitedItemsProvider)
		{
			InventoryManager inventoryManager = InventoryManager.CreateInstance();
			InventoryEventLogger eventLogger = new InventoryEventLogger(inventoryManager, adjustInventoryTracking);
			IUnlimitedItemsAnalytics unlimitedItemsAnalytics = new UnlimitedItemsAnalytics();
			IUnlimitedItems unlimitedItems = new UnlimitedItems(inventoryManager, timingManager, unlimitedItemsProvider, unlimitedItemsAnalytics);
			return new InventorySystem(inventoryManager, eventLogger, unlimitedItems);
		}
	}
}
