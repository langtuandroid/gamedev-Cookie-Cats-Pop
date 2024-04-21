using System;
using System.Collections;
using System.Collections.Generic;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.Rewards.Assets;
using TactileModules.GameCore.UI;

namespace TactileModules.GameCore.Rewards
{
	public class GiveAndAnimateRewards : IGiveAndAnimateRewards
	{
		public GiveAndAnimateRewards(IVisualInventory visualInventory, IUIController uiController, IRewardsFactory rewardsFactory, IAssetModel assets)
		{
			this.visualInventory = visualInventory;
			this.uiController = uiController;
			this.rewardsFactory = rewardsFactory;
			this.assets = assets;
		}

		public IEnumerator AddToInventory(List<ItemAmount> rewards, string analyticsTag)
		{
			yield break;
		}

		public IEnumerator Gifts(List<ItemAmount> rewards, int giftType, string analyticsTag)
		{
			using (IInventoryItemAnimator animator = this.visualInventory.CreateAnimator(new Func<InventoryItem, int, bool>(this.RewardFilter)))
			{
				this.GiveRewards(rewards, analyticsTag);
				GiftRewardsView view = this.uiController.ShowView<GiftRewardsView>(this.assets.GiftRewardsView);
				view.Initialize();
				view.HideButtons();
				IAddBoostersToInventory addBoostersToInventory = this.rewardsFactory.CreateAddBoostersToInventory();
				addBoostersToInventory.ShowView();
				addBoostersToInventory.HideBoosterContainer();
				addBoostersToInventory.RewardGrid.Initialize(this.visualInventory.InventoryManager, rewards, true);
				addBoostersToInventory.HideRewardGrid();
				yield return view.InitializeVisualGift(giftType);
				while (!view.OpenGift)
				{
					yield return null;
				}
				yield return view.AnimateOpenGift();
				yield return FiberHelper.RunParallel(new IEnumerator[]
				{
					view.AnimateRewardsIn(),
					addBoostersToInventory.AnimateRewardGridIn()
				});
				yield return addBoostersToInventory.AnimateInventoryRewards(animator, rewards, false);
				yield return FiberHelper.Wait(0.7f, (FiberHelper.WaitFlag)0);
				yield return FiberHelper.RunParallel(new IEnumerator[]
				{
					addBoostersToInventory.AnimateRewardGridOut(),
					view.AnimateRewardsOut()
				});
				addBoostersToInventory.CloseView();
				view.Close(0);
			}
			yield break;
		}

		private void GiveRewards(List<ItemAmount> rewards, string analyticsTag)
		{
			foreach (ItemAmount itemAmount in rewards)
			{
				this.visualInventory.InventoryManager.Add(itemAmount.ItemId, itemAmount.Amount, analyticsTag);
			}
		}

		private bool RewardFilter(InventoryItem item, int amount)
		{
			return amount > 0;
		}

		private readonly IVisualInventory visualInventory;

		private readonly IUIController uiController;

		private readonly IAssetModel assets;

		private readonly IRewardsFactory rewardsFactory;
	}
}
