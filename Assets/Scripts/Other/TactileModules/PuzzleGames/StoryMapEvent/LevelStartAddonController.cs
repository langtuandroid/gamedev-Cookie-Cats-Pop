using System;
using System.Collections;
using Fibers;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class LevelStartAddonController
	{
		public LevelStartAddonController(CurrencyCollector currencyCollector, CurrencyAvailability currencyAvailability, StoryMapEventActivation featureActivation, IFlowFactory flowFactory, IFlowStack flowStack, StoryMapEventLevelStartAddon storyMapEventLevelStartAddon)
		{
			this.currencyCollector = currencyCollector;
			this.currencyAvailability = currencyAvailability;
			this.featureActivation = featureActivation;
			this.flowFactory = flowFactory;
			this.flowStack = flowStack;
			this.storyMapEventLevelStartAddon = storyMapEventLevelStartAddon;
			this.storyMapEventLevelStartAddon.OnButtonClicked += this.StoryMapButtonClicked;
		}

		private void StoryMapButtonClicked()
		{
			new Fiber(this.CloseAndStartStoryMapFlow());
		}

		private IEnumerator CloseAndStartStoryMapFlow()
		{
			UIView view = this.storyMapEventLevelStartAddon.GetComponentInParent<UIView>();
			view.Close(0);
			while (!(this.flowStack.Top is MainMapFlow))
			{
				yield return null;
			}
			this.flowFactory.CreateAndPushStoryMapFlow();
			yield break;
		}

		public void ShowAddonIfRequired()
		{
			bool shouldLevelAwardCurrency = this.currencyCollector.ShouldLevelAwardCurrency;
			this.storyMapEventLevelStartAddon.SetVisible(shouldLevelAwardCurrency);
			if (shouldLevelAwardCurrency)
			{
				this.storyMapEventLevelStartAddon.SetPagesLeft(this.currencyAvailability.GetCurrencyLeftToEarn());
				this.fiber.Start(this.UpdateTimeLeft());
			}
		}

		private IEnumerator UpdateTimeLeft()
		{
			int lastSecondsLeft = -1;
			for (;;)
			{
				int secondsLeft = this.featureActivation.GetSecondsLeft();
				if (secondsLeft != lastSecondsLeft)
				{
					this.storyMapEventLevelStartAddon.SetTimeLeft(this.featureActivation.GetSecondsLeft());
					lastSecondsLeft = secondsLeft;
				}
				yield return null;
			}
			yield break;
		}

		public void Teardown()
		{
			this.fiber.Terminate();
		}

		private readonly CurrencyCollector currencyCollector;

		private readonly CurrencyAvailability currencyAvailability;

		private readonly StoryMapEventActivation featureActivation;

		private readonly IFlowFactory flowFactory;

		private readonly IFlowStack flowStack;

		private readonly StoryMapEventLevelStartAddon storyMapEventLevelStartAddon;

		private readonly Fiber fiber = new Fiber();
	}
}
