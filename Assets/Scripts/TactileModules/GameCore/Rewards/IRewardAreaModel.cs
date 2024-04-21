using System;
using TactileModules.GameCore.Inventory;

namespace TactileModules.GameCore.Rewards
{
	public interface IRewardAreaModel
	{
		InventoryCollectTarget GetTargetPosition(InventoryItem item);

		void RegisterTarget(InventoryCollectTarget target);

		void UnregisterTarget(InventoryCollectTarget target);
	}
}
