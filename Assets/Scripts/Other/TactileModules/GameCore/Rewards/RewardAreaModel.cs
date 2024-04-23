using System;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;

namespace TactileModules.GameCore.Rewards
{
	public class RewardAreaModel : IRewardAreaModel
	{
		public InventoryCollectTarget GetTargetPosition(InventoryItem item)
		{
			if (this.targetStacks.ContainsKey(item))
			{
				int count = this.targetStacks[item].Count;
				return this.targetStacks[item][count - 1];
			}
			return null;
		}

		public void RegisterTarget(InventoryCollectTarget target)
		{
			if (!this.targetStacks.ContainsKey(target.TypeToCollect))
			{
				this.targetStacks.Add(target.TypeToCollect, new List<InventoryCollectTarget>
				{
					target
				});
			}
			else if (!this.targetStacks[target.TypeToCollect].Contains(target))
			{
				this.targetStacks[target.TypeToCollect].Add(target);
			}
		}

		public void UnregisterTarget(InventoryCollectTarget target)
		{
			if (this.targetStacks.ContainsKey(target.TypeToCollect))
			{
				if (!this.targetStacks[target.TypeToCollect].Contains(target))
				{
					return;
				}
				if (this.targetStacks[target.TypeToCollect].Count <= 1)
				{
					this.targetStacks.Remove(target.TypeToCollect);
				}
				else
				{
					this.targetStacks[target.TypeToCollect].Remove(target);
				}
			}
		}

		private Dictionary<InventoryItem, List<InventoryCollectTarget>> targetStacks = new Dictionary<InventoryItem, List<InventoryCollectTarget>>();
	}
}
