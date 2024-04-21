using System;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.PuzzleGame.Inventory;
using TactileModules.Timing.Interfaces;

namespace TactileModules.Inventory
{
	public static class InventorySystemBuilder
	{
		public static IInventorySystem Build(IAnalytics analytics, IAdjustInventoryTracking adjustInventoryTracking, ITimingManager timingManager, IUnlimitedItemsProvider unlimitedItemsProvider)
		{
			InventoryManager inventoryManager = InventoryManager.CreateInstance();
			analytics.RegisterDecorator(new InventoryManagerBasicEventDecorator(inventoryManager));
			InventoryEventLogger eventLogger = new InventoryEventLogger(inventoryManager, analytics, adjustInventoryTracking);
			IUnlimitedItemsAnalytics unlimitedItemsAnalytics = new UnlimitedItemsAnalytics(analytics);
			IUnlimitedItems unlimitedItems = new UnlimitedItems(inventoryManager, timingManager, unlimitedItemsProvider, unlimitedItemsAnalytics);
			return new InventorySystem(inventoryManager, eventLogger, unlimitedItems);
		}
	}
}
