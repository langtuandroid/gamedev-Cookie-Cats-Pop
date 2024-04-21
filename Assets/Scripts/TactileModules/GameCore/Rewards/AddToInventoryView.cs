using System;
using System.Collections.Generic;
using Tactile;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public class AddToInventoryView : ExtensibleView<IAddToInventoryViewExtension>
	{
		public RewardGrid RewardGrid
		{
			get
			{
				return this.rewardGrid;
			}
		}

		public void Initialize(InventoryManager inventoryManager, List<ItemAmount> rewards)
		{
			this.rewardGrid.Initialize(inventoryManager, rewards, this.overridePresetGridColumns);
			this.rewardGrid.ItemArea.Layout();
			if (base.Extension != null)
			{
				base.Extension.HandleElementsBasedOnRewards(inventoryManager, rewards);
			}
		}

		[SerializeField]
		private RewardGrid rewardGrid;

		[SerializeField]
		private bool overridePresetGridColumns;
	}
}
