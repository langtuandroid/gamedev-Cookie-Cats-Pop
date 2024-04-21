using System;
using System.Collections.Generic;
using TactileModules.ComponentLifecycle;
using TactileModules.PuzzleGames.GameCore;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class LevelStartAddonLifecycleHandler : ComponentLifecycleHandler<StoryMapEventLevelStartAddon>
	{
		public LevelStartAddonLifecycleHandler(CurrencyCollector currencyCollector, CurrencyAvailability currencyAvailability, StoryMapEventActivation featureActivation, IFlowFactory flowFactory, IFlowStack flowStack) : base(ComponentLifecycleHandler<StoryMapEventLevelStartAddon>.InitializationTiming.Start)
		{
			this.currencyCollector = currencyCollector;
			this.currencyAvailability = currencyAvailability;
			this.featureActivation = featureActivation;
			this.flowFactory = flowFactory;
			this.flowStack = flowStack;
		}

		protected override void InitializeComponent(StoryMapEventLevelStartAddon component)
		{
			LevelStartAddonController levelStartAddonController = new LevelStartAddonController(this.currencyCollector, this.currencyAvailability, this.featureActivation, this.flowFactory, this.flowStack, component);
			levelStartAddonController.ShowAddonIfRequired();
			this.controllers[component] = levelStartAddonController;
		}

		protected override void TeardownComponent(StoryMapEventLevelStartAddon component)
		{
			this.controllers[component].Teardown();
			this.controllers.Remove(component);
			base.TeardownComponent(component);
		}

		private readonly CurrencyCollector currencyCollector;

		private readonly CurrencyAvailability currencyAvailability;

		private readonly StoryMapEventActivation featureActivation;

		private readonly IFlowFactory flowFactory;

		private readonly IFlowStack flowStack;

		private readonly Dictionary<StoryMapEventLevelStartAddon, LevelStartAddonController> controllers = new Dictionary<StoryMapEventLevelStartAddon, LevelStartAddonController>();
	}
}
