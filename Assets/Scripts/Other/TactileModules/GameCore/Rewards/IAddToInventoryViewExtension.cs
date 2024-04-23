using System;
using System.Collections.Generic;
using Tactile;

namespace TactileModules.GameCore.Rewards
{
	public interface IAddToInventoryViewExtension
	{
		void HandleElementsBasedOnRewards(InventoryManager inventoryManager, List<ItemAmount> rewards);
	}
}
