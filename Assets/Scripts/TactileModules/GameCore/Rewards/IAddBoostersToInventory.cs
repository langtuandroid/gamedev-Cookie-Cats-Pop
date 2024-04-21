using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;

namespace TactileModules.GameCore.Rewards
{
	public interface IAddBoostersToInventory
	{
		RewardGrid RewardGrid { get; }

		void OverrideRewardGrid(RewardGrid rewardGrid);

		void ShowView();

		void CloseView();

		void HideRewardGrid();

		void HideBoosterContainer();

		IEnumerator AnimateRewardGridIn();

		IEnumerator AnimateRewardGridOut();

		IEnumerator AnimateInventoryRewards(IInventoryItemAnimator animator, List<ItemAmount> rewards, bool detach = false);
	}
}
