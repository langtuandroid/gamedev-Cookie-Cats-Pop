using System;
using TactileModules.ComponentLifecycle;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CurrencyRewardLifecycleHandler : ComponentLifecycleHandler<StoryMapEventCurrencyReward>
	{
		public CurrencyRewardLifecycleHandler(CurrencyCollector currencyCollector) : base(ComponentLifecycleHandler<StoryMapEventCurrencyReward>.InitializationTiming.Awake)
		{
			this.currencyCollector = currencyCollector;
		}

		protected override void InitializeComponent(StoryMapEventCurrencyReward component)
		{
			if (this.currencyCollector.ShouldLevelAwardCurrency)
			{
				FiberCtrl.Pool.Run(component.PlayCurrencyCollect(), false);
			}
		}

		protected override void TeardownComponent(StoryMapEventCurrencyReward component)
		{
			base.TeardownComponent(component);
		}

		private readonly CurrencyCollector currencyCollector;
	}
}
