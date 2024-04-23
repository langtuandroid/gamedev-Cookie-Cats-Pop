using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGame.SlidesAndLadders.UI;
using TactileModules.PuzzleGame.SlidesAndLadders.Views;
using UnityEngine;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Controllers
{
	public class SlidesAndLaddersRewardController
	{
		public SlidesAndLaddersRewardController(ISlidesAndLaddersRewards featureRewards, ISlidesAndLaddersInventory inventory, IDataProvider<SlidesAndLaddersConfig> config)
		{
			this.featureRewards = featureRewards;
			this.inventory = inventory;
			this.config = config;
		}

		public void AddWheelRewardToInventory(WheelSlot wheelSlot)
		{
			List<ItemAmount> rewards = new List<ItemAmount>
			{
				new ItemAmount
				{
					ItemId = wheelSlot.item.ToString(),
					Amount = 1
				}
			};
			this.diceRewardViewFiber.Start(this.ShowWheelRewardView(rewards));
			this.AddRewardsToInventory(rewards, "snlWheelReward");
		}

		private IEnumerator ShowWheelRewardView(List<ItemAmount> rewards)
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersWheelRewardView> state = UIViewManager.Instance.ShowView<SlidesAndLaddersWheelRewardView>(new object[0]);
			state.View.Initialize(rewards);
			yield return state.WaitForClose();
			yield break;
		}

		public IEnumerator CheckForLevelRewards(int levelIndex)
		{
			if (this.featureRewards.LevelHasValidChest(levelIndex))
			{
				yield return this.AddLevelRewardsToInventory(1, levelIndex);
			}
			yield break;
		}

		private IEnumerator AddLevelRewardsToInventory(int rewardsAmount, int levelIndex)
		{
			if (!this.featureRewards.HasClaimedLevelRewardAtIndex(levelIndex))
			{
				List<ItemAmount> rewards = this.GetRandomLevelRewards(rewardsAmount);
				this.featureRewards.ClaimLevelRewardAtIndex(levelIndex);
				int chestRank = this.featureRewards.GetChestRank(levelIndex);
				this.AddRewardsToInventory(rewards, "snlChestReward" + chestRank.ToString());
				if (SlidesAndLaddersRewardController.OnChestRewardsReceived != null)
				{
					SlidesAndLaddersRewardController.OnChestRewardsReceived(rewards, chestRank);
				}
				UIViewManager.UIViewState vs = this.ShowRewardView(rewards);
				yield return vs.WaitForClose();
			}
			yield break;
		}

		private UIViewManager.UIViewState ShowRewardView(List<ItemAmount> rewards)
		{
			UIViewManager.UIViewStateGeneric<SlidesAndLaddersRewardView> uiviewStateGeneric = UIViewManager.Instance.ShowView<SlidesAndLaddersRewardView>(new object[0]);
			uiviewStateGeneric.View.Initialize(rewards);
			return uiviewStateGeneric;
		}

		public UIViewManager.UIViewState AddFeatureRewardsToInventory()
		{
			List<ItemAmount> rewards = this.featureRewards.GetFeatureRewards();
			this.AddRewardsToInventory(rewards, "snlFinalChestReward");
			return this.ShowRewardView(rewards);
		}

		public IEnumerator AddSlideRewardsToFeatureRewards(MapStreamer mapStreamer, SlidesAndLaddersSpline[] splines, int preSlideIndex, int currentLevelIndex, int lastLevelIndex)
		{
			List<ItemAmount> rewards = this.featureRewards.GetRandomSlidesRewards(1);
			this.featureRewards.AddChestRewards(rewards);
			yield return this.ShowSlideRewardsView(rewards, mapStreamer, splines, preSlideIndex, currentLevelIndex, lastLevelIndex);
			yield break;
		}

		private IEnumerator ShowSlideRewardsView(List<ItemAmount> rewards, MapStreamer mapStreamer, SlidesAndLaddersSpline[] splines, int preSlideIndex, int currentLevelIndex, int lastLevelIndex)
		{
			foreach (SlidesAndLaddersSpline spline in splines)
			{
				if (spline.name.Contains(SlidesAndLaddersHelperMethods.GetIdForSlide(preSlideIndex + 1)))
				{
					UIViewManager.UIViewState state = UIViewManager.Instance.ShowView<SlidesAndLaddersSlidesRewardView>(new object[0]);
					yield return ((SlidesAndLaddersSlidesRewardView)state.View).Animate(rewards, mapStreamer, spline, preSlideIndex, currentLevelIndex, lastLevelIndex);
					yield return state.WaitForClose();
					yield break;
				}
			}
			yield break;
		}

		private void AddRewardsToInventory(List<ItemAmount> rewards, string analyticsId = "")
		{
			this.inventory.AddToInventory(rewards, analyticsId);
		}

		private List<ItemAmount> GetRandomLevelRewards(int amount)
		{
			List<ItemAmount> list = new List<ItemAmount>();
			for (int i = 0; i < amount; i++)
			{
				list.Add(this.config.Get().LevelRewards[UnityEngine.Random.Range(0, this.config.Get().LevelRewards.Count)]);
			}
			return list;
		}

		public static SlidesAndLaddersRewardController.OnChestRewardsReceivedEvent OnChestRewardsReceived;

		private readonly ISlidesAndLaddersRewards featureRewards;

		private readonly ISlidesAndLaddersInventory inventory;

		private readonly IDataProvider<SlidesAndLaddersConfig> config;

		private Fiber diceRewardViewFiber = new Fiber();

		public delegate void OnChestRewardsReceivedEvent(List<ItemAmount> rewards, int chestRank);
	}
}
