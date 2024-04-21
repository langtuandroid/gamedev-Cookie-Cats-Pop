using System;
using System.Collections.Generic;
using TactileModules.ComponentLifecycle;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CountdownLifecycleHandler : ComponentLifecycleHandler<StoryMapEventCountdown>
	{
		public CountdownLifecycleHandler(StoryMapEventActivation featureActivation) : base(ComponentLifecycleHandler<StoryMapEventCountdown>.InitializationTiming.Awake)
		{
			this.featureActivation = featureActivation;
		}

		protected override void InitializeComponent(StoryMapEventCountdown component)
		{
			CountdownController value = new CountdownController(this.featureActivation, component);
			this.controllers[component] = value;
		}

		protected override void TeardownComponent(StoryMapEventCountdown component)
		{
			this.controllers[component].Teardown();
			this.controllers.Remove(component);
			base.TeardownComponent(component);
		}

		private readonly StoryMapEventActivation featureActivation;

		private readonly Dictionary<StoryMapEventCountdown, CountdownController> controllers = new Dictionary<StoryMapEventCountdown, CountdownController>();
	}
}
