using System;
using TactileModules.PuzzleGame.SlidesAndLadders.Controllers;
using TactileModules.PuzzleGame.SlidesAndLadders.Data;
using TactileModules.PuzzleGame.SlidesAndLadders.Model;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Analytics
{
	public class SlidesAndLaddersWheelAnalytics
	{
		public SlidesAndLaddersWheelAnalytics(IFlowStack flowStack, ISlidesAndLaddersLevelDatabase levelDatabase, ISlidesAndLaddersFeatureProgression featureProgression, ISlidesAndLaddersRewards rewards)
		{
			this.levelDatabase = levelDatabase;
			this.featureProgression = featureProgression;
			this.rewards = rewards;
			flowStack.Changed += this.HandleFlowChanged;
		}

		private void HandleFlowChanged(IFlow newFlow, IFlow oldFlow)
		{
			SlidesAndLaddersMapFlow slidesAndLaddersMapFlow = oldFlow as SlidesAndLaddersMapFlow;
			if (slidesAndLaddersMapFlow != null)
			{
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow2 = slidesAndLaddersMapFlow;
				slidesAndLaddersMapFlow2.OnWheelSpinResult = (SlidesAndLaddersMapFlow.OnWheelSpinResultEvent)Delegate.Remove(slidesAndLaddersMapFlow2.OnWheelSpinResult, new SlidesAndLaddersMapFlow.OnWheelSpinResultEvent(this.WheelSpinResult));
			}
			SlidesAndLaddersMapFlow slidesAndLaddersMapFlow3 = newFlow as SlidesAndLaddersMapFlow;
			if (slidesAndLaddersMapFlow3 != null)
			{
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow4 = slidesAndLaddersMapFlow3;
				slidesAndLaddersMapFlow4.OnWheelSpinResult = (SlidesAndLaddersMapFlow.OnWheelSpinResultEvent)Delegate.Combine(slidesAndLaddersMapFlow4.OnWheelSpinResult, new SlidesAndLaddersMapFlow.OnWheelSpinResultEvent(this.WheelSpinResult));
			}
		}

		private void WheelSpinResult(WheelSlot result, int levelIndexBeforeSpin, int levelIndexAfterSpin, int featureSpinCount)
		{
			ILevelProxy levelProxy = this.levelDatabase.GetLevelProxy(levelIndexAfterSpin);
			this.context = levelProxy.AnalyticsDescriptors[0];
			this.levelNumber = levelIndexAfterSpin;
			this.mapStepsMovedUp = levelIndexAfterSpin - levelIndexBeforeSpin;
			this.ladderReached = this.levelDatabase.IsLadderLevel(levelIndexAfterSpin);
			this.slideReached = this.levelDatabase.IsSlideLevel(levelIndexAfterSpin);
			this.spinCount = featureSpinCount;
			this.wheelResult = ((!result.IsReward()) ? result.stepsToAdd.ToString() : result.item.ToString());
			this.chestReached = (this.levelDatabase.IsTreasureLevel(levelIndexAfterSpin) && !this.rewards.HasClaimedLevelRewardAtIndex(levelIndexAfterSpin));
			this.chestRank = ((!this.chestReached) ? 0 : this.levelDatabase.GetChestRank(levelIndexAfterSpin));
			string text = string.Empty;
			foreach (ItemAmount itemAmount in this.rewards.GetFeatureRewards())
			{
				string str = string.IsNullOrEmpty(text) ? string.Empty : ",";
				text = text + str + itemAmount.ItemId;
				if (itemAmount.ItemId == "Coin")
				{
					this.finalChestCoinsAmount = itemAmount.Amount;
				}
			}
			if (this.featureProgression.IsLevelIndexEndChest(levelIndexAfterSpin))
			{
				this.finalChestReward = text;
			}
			this.LogSlidesAndLaddersWheelSpunEvent();
		}

		public void LogSlidesAndLaddersWheelSpunEvent()
		{
			TactileAnalytics.Instance.LogEvent(new SlidesAndLaddersWheelSpunEvent(this.levelNumber, this.wheelResult, this.mapStepsMovedUp, this.chestReached, this.chestRank, this.spinCount, this.ladderReached, this.slideReached, this.finalChestCoinsAmount, this.finalChestReward), -1.0, null);
		}

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;

		private readonly ISlidesAndLaddersRewards rewards;

		private readonly ISlidesAndLaddersFeatureProgression featureProgression;

		private string wheelResult;

		private string context;

		private string finalChestReward;

		private bool chestReached;

		private bool ladderReached;

		private bool slideReached;

		private int chestRank;

		private int levelNumber;

		private int mapStepsMovedUp;

		private int spinCount;

		private int finalChestCoinsAmount;
	}
}
