using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public class RewardSystem
	{
		public RewardSystem(IUIController uiController, IVisualInventory visualInventory, IRewardAreaController rewardAreaController, IRewardAreaModel rewardAreaModel, IAssetModel assets, IRewardsFactory rewardsFactory)
		{
			this.uiController = uiController;
			this.visualInventory = visualInventory;
			this.rewardAreaController = rewardAreaController;
			this.rewardAreaModel = rewardAreaModel;
			this.assets = assets;
			this.rewardsFactory = rewardsFactory;
		}

		public IRewardsFactory GetRewardsFactory
		{
			get
			{
				return this.rewardsFactory;
			}
		}

		public IEnumerator GiveAndAnimateRewards(List<ItemAmount> rewards, RewardGrid rewardGrid, string analyticsTag, bool detach = false)
		{
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.RewardFilter)))
			{
				this.GiveRewards(rewards, analyticsTag);
				yield return this.AnimateRewardGrid(animator, rewards, rewardGrid, detach);
			}
			yield break;
		}

		public IEnumerator GiveAndShowRewardView(RewardView rewardViewPrefab, List<ItemAmount> rewards, string analyticsTag)
		{
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.RewardFilter)))
			{
				this.GiveRewards(rewards, analyticsTag);
				RewardView view = this.uiController.ShowView<RewardView>(rewardViewPrefab);
				view.RewardGrid.Initialize(this.visualInventory.InventoryManager, rewards, true);
				bool okClicked = false;
				view.OkClicked += delegate()
				{
					okClicked = true;
				};
				while (!okClicked)
				{
					yield return null;
				}
				yield return FiberHelper.RunParallel(new IEnumerator[]
				{
					FiberHelper.RunDelayed(0f, delegate
					{
						view.Close(0);
					}),
					this.AnimateRewardGrid(animator, rewards, view.RewardGrid, true)
				});
			}
			yield break;
		}

		public IEnumerator GiveAndShowAddToInventoryView(List<ItemAmount> rewards, string analyticsTag)
		{
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.RewardFilter)))
			{
				this.GiveRewards(rewards, analyticsTag);
				AddToInventoryView view = this.uiController.ShowView<AddToInventoryView>(this.assets.AddToInventoryView);
				view.Initialize(this.visualInventory.InventoryManager, rewards);
				yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
				yield return this.AnimateRewardGrid(animator, rewards, view.RewardGrid, false);
				yield return FiberHelper.Wait(1f, (FiberHelper.WaitFlag)0);
				view.Close(0);
			}
			yield break;
		}

		private bool RewardFilter(InventoryItem item, int amount)
		{
			return amount > 0;
		}

		private IEnumerator AnimateRewardGrid(IInventoryItemAnimator animator, List<ItemAmount> rewards, RewardGrid rewardGrid, bool detach = false)
		{
			if (detach)
			{
				rewardGrid.transform.parent = null;
				rewardGrid.gameObject.SetLayerRecursively(rewardGrid.gameObject.layer - 1);
			}
			this.rewardGrid = rewardGrid;
			this.rewardGrid.ItemArea.enabled = false;
			yield return animator.Animate(new Func<InventoryItem, Vector3>(this.GetRewardGridPosition));
			if (detach)
			{
				UnityEngine.Object.Destroy(rewardGrid.gameObject);
			}
			yield break;
		}

		private Vector3 GetRewardGridPosition(InventoryItem item)
		{
			if (this.rewardGrid != null)
			{
				return this.rewardGrid.GetSlotPosition(item);
			}
			return Vector3.zero;
		}

		private void GiveRewards(List<ItemAmount> rewards, string analyticsTag)
		{
			foreach (ItemAmount itemAmount in rewards)
			{
				this.visualInventory.InventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, analyticsTag);
			}
		}

		private readonly IUIController uiController;

		private readonly IVisualInventory visualInventory;

		private readonly IRewardAreaController rewardAreaController;

		private readonly IRewardAreaModel rewardAreaModel;

		private readonly IAssetModel assets;

		private readonly IRewardsFactory rewardsFactory;

		private RewardGrid rewardGrid;
	}
}
