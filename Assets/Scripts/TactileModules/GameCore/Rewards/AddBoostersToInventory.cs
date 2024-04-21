using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.Rewards
{
	public class AddBoostersToInventory : IAddBoostersToInventory
	{
		public AddBoostersToInventory(IUIController uiController, IAssetModel assets)
		{
			this.uiController = uiController;
			this.assets = assets;
		}

		public RewardGrid RewardGrid { get; private set; }

		public void OverrideRewardGrid(RewardGrid rewardGrid)
		{
			this.RewardGrid = rewardGrid;
		}

		public void ShowView()
		{
			this.view = this.uiController.ShowView<InventoryBoostersView>(this.assets.InventoryBoostersView);
			this.RewardGrid = this.view.RewardGrid;
		}

		public void CloseView()
		{
			this.view.Close(0);
		}

		public void HideRewardGrid()
		{
			this.view.HideRewardGrid();
		}

		public void HideBoosterContainer()
		{
			this.view.HideBoosterContainer();
		}

		public IEnumerator AnimateRewardGridIn()
		{
			yield return this.view.AnimateRewardGridIn();
			yield break;
		}

		public IEnumerator AnimateRewardGridOut()
		{
			yield return this.view.AnimateRewardGridOut();
			yield break;
		}

		public IEnumerator AnimateInventoryRewards(IInventoryItemAnimator animator, List<ItemAmount> rewards, bool detach = false)
		{
			if (this.RewardGrid != null)
			{
				this.view.RewardGrid = this.RewardGrid;
			}
			IAnimateRewardGrid animateRewardGrid = this.CreateAnimateRewardGrid(animator, this.view.RewardGrid, rewards, detach);
			yield return animateRewardGrid.Animate();
			yield break;
		}

		private IAnimateRewardGrid CreateAnimateRewardGrid(IInventoryItemAnimator animator, RewardGrid rewardGrid, List<ItemAmount> reward, bool detach = false)
		{
			return new AnimateRewardGrid(animator, this.view.RewardGrid, reward, detach);
		}

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private InventoryBoostersView view;
	}
}
