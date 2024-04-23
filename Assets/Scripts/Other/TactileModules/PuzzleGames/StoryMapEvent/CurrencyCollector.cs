using System;
using System.Collections;
using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.PuzzleCore.LevelPlaying;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CurrencyCollector
	{
		public CurrencyCollector(IPlayFlowEvents playFlowEvents, InventoryManager inventoryManager, ICurrencyAvailability currencyAvailability, IStoryManager storyManager)
		{
			this.inventoryManager = inventoryManager;
			this.currencyAvailability = currencyAvailability;
			this.storyManager = storyManager;
			playFlowEvents.PlayFlowCreated += this.HandlePlayFlowCreated;
		}

		public bool ShouldLevelAwardCurrency { get; private set; }

		private void HandlePlayFlowCreated(ICorePlayFlow flow)
		{
			flow.LevelSessionStarted += this.HandleLevelSessionStarted;
			flow.LevelEndedHook.Register(new Func<ILevelAttempt, IEnumerator>(this.HandleLevelEnded));
			flow.ResultsShownHook.Register(new Func<ICorePlayFlow, PostLevelPlayedAction, IEnumerator>(this.HandleResultsShown));
		}

		private void HandleLevelSessionStarted(ILevelSessionRunner levelSessionRunner)
		{
			int index = levelSessionRunner.LevelProxy.Index;
			this.ShouldLevelAwardCurrency = this.currencyAvailability.ShouldLevelAwardStoryCurrency(index);
		}

		private IEnumerator HandleLevelEnded(ILevelAttempt attempt)
		{
			if (this.ShouldLevelAwardCurrency && attempt.Completed)
			{
				this.inventoryManager.Add("Star", 1, "LevelCompleted");
				this.storyManager.TotalPagesCollected++;
			}
			yield break;
		}

		private IEnumerator HandleResultsShown(ICorePlayFlow corePlayFlow, PostLevelPlayedAction postLevelPlayedAction)
		{
			this.ShouldLevelAwardCurrency = false;
			yield break;
		}

		private readonly InventoryManager inventoryManager;

		private readonly ICurrencyAvailability currencyAvailability;

		private readonly IStoryManager storyManager;
	}
}
