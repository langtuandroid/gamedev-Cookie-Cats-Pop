using System;
using Tactile;
using TactileModules.GameCore.Audio;
using TactileModules.GameCore.Rewards;

namespace TactileModules.GameCore.Inventory
{
	public class InventorySystemBuilder
	{
		public static InventorySystem Build(IRewardAreaModel rewardAreaModel, InventoryManager inventoryManager, AudioDatabaseInjector audio)
		{
			VisualInventory visualInventory = new VisualInventory(rewardAreaModel, audio, inventoryManager);
			return new InventorySystem(visualInventory);
		}
	}
}
