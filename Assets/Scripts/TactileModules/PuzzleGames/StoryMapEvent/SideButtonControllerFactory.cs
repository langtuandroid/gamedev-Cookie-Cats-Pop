using System;
using System.Collections.Generic;
using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.SideMapButtons;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class SideButtonControllerFactory : ISideMapButtonControllerProvider
	{
		public SideButtonControllerFactory(IViewFactory viewFactory, FlowFactory flowFactory, IStoryManager storyManager, InventoryManager inventoryManager, StoryMapEventActivation featureActivation, ReminderCooldown reminderCooldown)
		{
			this.viewFactory = viewFactory;
			this.flowFactory = flowFactory;
			this.storyManager = storyManager;
			this.inventoryManager = inventoryManager;
			this.featureActivation = featureActivation;
			this.reminderCooldown = reminderCooldown;
		}

		public List<ISideMapButtonController> CreateButtonControllers()
		{
			return new List<ISideMapButtonController>
			{
				new SideButtonController(this.viewFactory, this.flowFactory, this.storyManager, this.inventoryManager, this.featureActivation, this.reminderCooldown)
			};
		}

		private readonly IViewFactory viewFactory;

		private readonly FlowFactory flowFactory;

		private readonly IStoryManager storyManager;

		private readonly InventoryManager inventoryManager;

		private readonly StoryMapEventActivation featureActivation;

		private readonly ReminderCooldown reminderCooldown;
	}
}
