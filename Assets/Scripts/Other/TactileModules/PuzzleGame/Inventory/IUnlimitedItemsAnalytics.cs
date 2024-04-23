using System;
using Tactile;

namespace TactileModules.PuzzleGame.Inventory
{
	public interface IUnlimitedItemsAnalytics
	{
		void LogUnlimitedItemActivated(InventoryManager.ItemChangeInfo info, int previouslyAvailableUnlimitedSeconds);
	}
}
