using System;
using System.Collections.Generic;
using TactileModules.GameCore.Rewards;
using UnityEngine;

namespace Tactile.GardenGame.Story.Rewards
{
	public class StoryRewardIndicator : RewardIndicatorBase
	{
		public StoryConfig.Reward Reward { get; private set; }

		public void Initialize(StoryConfig.Reward reward, InventoryManager inventoryManager, int rewardIndex)
		{
			this.rewardGrid.Initialize(reward.Items, false);
			rewardIndex = Mathf.Clamp(rewardIndex, 0, this.spriteNames.Count - 1);
			this.rewardSprite.SpriteName = this.spriteNames[rewardIndex];
			this.tooltipPivot.gameObject.SetActive(false);
			this.Reward = reward;
		}

		[SerializeField]
		private UISprite rewardSprite;

		[SerializeField]
		private List<string> spriteNames;

		[SerializeField]
		private global::RewardGrid rewardGrid;
	}
}
