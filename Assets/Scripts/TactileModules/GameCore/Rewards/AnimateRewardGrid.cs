using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public class AnimateRewardGrid : IAnimateRewardGrid
	{
		public AnimateRewardGrid(IInventoryItemAnimator animator, RewardGrid rewardGrid, List<ItemAmount> reward, bool detach = false)
		{
			this.animator = animator;
			this.reward = reward;
			this.rewardGrid = rewardGrid;
			this.detach = detach;
		}

		public IEnumerator Animate()
		{
			if (this.detach)
			{
				this.rewardGrid.transform.parent = null;
				this.rewardGrid.gameObject.SetLayerRecursively(this.rewardGrid.gameObject.layer - 1);
			}
			this.rewardGrid.ItemArea.enabled = false;
			yield return this.animator.Animate(new Func<InventoryItem, Vector3>(this.GetRewardGridPosition));
			if (this.detach)
			{
				UnityEngine.Object.Destroy(this.rewardGrid.gameObject);
			}
			yield break;
		}

		private Vector3 GetRewardGridPosition(InventoryItem item)
		{
			if (this.rewardGrid != null)
			{
				Vector3 slotPosition = this.rewardGrid.GetSlotPosition(item);
				this.rewardGrid.RemoveSlotItem(item);
				return slotPosition;
			}
			return Vector3.zero;
		}

		private readonly IInventoryItemAnimator animator;

		private readonly RewardGrid rewardGrid;

		private readonly List<ItemAmount> reward;

		private readonly bool detach;
	}
}
