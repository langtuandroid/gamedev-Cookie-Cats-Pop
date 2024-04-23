using System;
using System.Collections.Generic;
using TactileModules.PuzzleGame.SlidesAndLadders.Controllers;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGame.SlidesAndLadders.Analytics
{
	public class SlidesAndLaddersGameSessionAnalytics : IMapPlugin
	{
		public SlidesAndLaddersGameSessionAnalytics(IFlowStack flowStack, ISlidesAndLaddersLevelDatabase levelDatabase)
		{
			this.levelDatabase = levelDatabase;
			SlidesAndLaddersRewardController.OnChestRewardsReceived = (SlidesAndLaddersRewardController.OnChestRewardsReceivedEvent)Delegate.Combine(SlidesAndLaddersRewardController.OnChestRewardsReceived, new SlidesAndLaddersRewardController.OnChestRewardsReceivedEvent(this.ChestRewardsReceived));
			flowStack.Changed += this.HandleFlowChanged;
			this.rewards = new List<ItemAmount>();
		}

		private void HandleFlowChanged(IFlow newFlow, IFlow oldFlow)
		{
			SlidesAndLaddersMapFlow slidesAndLaddersMapFlow = oldFlow as SlidesAndLaddersMapFlow;
			if (slidesAndLaddersMapFlow != null)
			{
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow2 = slidesAndLaddersMapFlow;
				slidesAndLaddersMapFlow2.OnGameSessionReceived = (SlidesAndLaddersMapFlow.OnGameSessionReceivedEvent)Delegate.Remove(slidesAndLaddersMapFlow2.OnGameSessionReceived, new SlidesAndLaddersMapFlow.OnGameSessionReceivedEvent(this.SetSessionId));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow3 = slidesAndLaddersMapFlow;
				slidesAndLaddersMapFlow3.OnLaddderUsed = (SlidesAndLaddersMapFlow.OnLadderUsedEvent)Delegate.Remove(slidesAndLaddersMapFlow3.OnLaddderUsed, new SlidesAndLaddersMapFlow.OnLadderUsedEvent(this.LadderUsed));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow4 = slidesAndLaddersMapFlow;
				slidesAndLaddersMapFlow4.OnSlideUsed = (SlidesAndLaddersMapFlow.OnSlideUsedEvent)Delegate.Remove(slidesAndLaddersMapFlow4.OnSlideUsed, new SlidesAndLaddersMapFlow.OnSlideUsedEvent(this.SlideUsed));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow5 = slidesAndLaddersMapFlow;
				slidesAndLaddersMapFlow5.OnLevelResultFlowCompleted = (SlidesAndLaddersMapFlow.OnLevelResultCompletedEvent)Delegate.Remove(slidesAndLaddersMapFlow5.OnLevelResultFlowCompleted, new SlidesAndLaddersMapFlow.OnLevelResultCompletedEvent(this.LogSlidesAndLaddersPlayedEvent));
			}
			SlidesAndLaddersMapFlow slidesAndLaddersMapFlow6 = newFlow as SlidesAndLaddersMapFlow;
			if (slidesAndLaddersMapFlow6 != null)
			{
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow7 = slidesAndLaddersMapFlow6;
				slidesAndLaddersMapFlow7.OnGameSessionReceived = (SlidesAndLaddersMapFlow.OnGameSessionReceivedEvent)Delegate.Combine(slidesAndLaddersMapFlow7.OnGameSessionReceived, new SlidesAndLaddersMapFlow.OnGameSessionReceivedEvent(this.SetSessionId));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow8 = slidesAndLaddersMapFlow6;
				slidesAndLaddersMapFlow8.OnLaddderUsed = (SlidesAndLaddersMapFlow.OnLadderUsedEvent)Delegate.Combine(slidesAndLaddersMapFlow8.OnLaddderUsed, new SlidesAndLaddersMapFlow.OnLadderUsedEvent(this.LadderUsed));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow9 = slidesAndLaddersMapFlow6;
				slidesAndLaddersMapFlow9.OnSlideUsed = (SlidesAndLaddersMapFlow.OnSlideUsedEvent)Delegate.Combine(slidesAndLaddersMapFlow9.OnSlideUsed, new SlidesAndLaddersMapFlow.OnSlideUsedEvent(this.SlideUsed));
				SlidesAndLaddersMapFlow slidesAndLaddersMapFlow10 = slidesAndLaddersMapFlow6;
				slidesAndLaddersMapFlow10.OnLevelResultFlowCompleted = (SlidesAndLaddersMapFlow.OnLevelResultCompletedEvent)Delegate.Combine(slidesAndLaddersMapFlow10.OnLevelResultFlowCompleted, new SlidesAndLaddersMapFlow.OnLevelResultCompletedEvent(this.LogSlidesAndLaddersPlayedEvent));
			}
		}

		private void SetSessionId(string sessionId, int levelNumber)
		{
			this.sessionId = sessionId;
			this.levelNumber = levelNumber;
			ILevelProxy levelProxy = this.levelDatabase.GetLevelProxy(levelNumber);
			this.context = levelProxy.AnalyticsDescriptors[0];
		}

		private void ChestRewardsReceived(List<ItemAmount> rewards, int chestRank)
		{
			this.rewards = rewards;
			this.chestRank = chestRank;
		}

		private void SlideUsed(int mapStepsMovedDown)
		{
			this.slideUsed = true;
			this.mapStepsMovedDown = mapStepsMovedDown;
		}

		private void LadderUsed(int mapStepsMovedUp)
		{
			this.ladderUsed = true;
			this.mapStepsMovedUp = mapStepsMovedUp;
		}

		private void LogSlidesAndLaddersPlayedEvent()
		{
			if (!string.IsNullOrEmpty(this.sessionId))
			{
				SlidesAndLaddersPlayedEvent slidesAndLaddersPlayedEvent = new SlidesAndLaddersPlayedEvent(this.levelNumber, this.ladderUsed, this.slideUsed, this.mapStepsMovedUp, this.mapStepsMovedDown, this.chestRank, (this.rewards.Count <= 0) ? null : this.rewards[0].ItemId, (this.rewards.Count <= 0) ? 0 : this.rewards[0].Amount);
				slidesAndLaddersPlayedEvent.SetLevelPlayingParameters(this.sessionId);
				TactileAnalytics.Instance.LogEvent(slidesAndLaddersPlayedEvent, -1.0, null);
			}
		}

		public void ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow)
		{
			if (mapFlow is SlidesAndLaddersMapFlow)
			{
			}
		}

		public void ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent)
		{
		}

		private readonly ISlidesAndLaddersLevelDatabase levelDatabase;

		private string sessionId;

		private string context;

		private bool completed;

		private bool slideUsed;

		private bool ladderUsed;

		private int levelNumber;

		private int mapStepsMovedDown;

		private int mapStepsMovedUp;

		private int chestRank;

		private List<ItemAmount> rewards;
	}
}
