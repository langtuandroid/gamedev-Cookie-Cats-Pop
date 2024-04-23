using System;
using System.Collections;
using Fibers;
using Tactile;
using Tactile.GardenGame.Story;
using TactileModules.SideMapButtons;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class SideButtonController : ISideMapButtonController
	{
		public SideButtonController(IViewFactory viewFactory, IFlowFactory flowFactory, IStoryManager storyManager, InventoryManager inventoryManager, StoryMapEventActivation featureActivation, ReminderCooldown reminderCooldown)
		{
			this.flowFactory = flowFactory;
			this.featureActivation = featureActivation;
			this.reminderCooldown = reminderCooldown;
			this.button = viewFactory.CreateSideButton();
			this.button.Clicked += this.ButtonOnClicked;
			this.fiber.Start(this.UpdateTimer());
			this.button.SetBadgeText((!storyManager.HasStarsToCompleteATask()) ? null : "!");
		}

		private IEnumerator UpdateTimer()
		{
			int lastSecondsLeft = -1;
			for (;;)
			{
				int secondsLeft = this.featureActivation.GetSecondsLeft();
				if (secondsLeft != lastSecondsLeft)
				{
					this.button.SetTimeLeft(this.featureActivation.GetSecondsLeft());
					lastSecondsLeft = secondsLeft;
				}
				yield return null;
			}
			yield break;
		}

		public bool VisibilityChecker(object data)
		{
			return this.featureActivation.HasActiveFeature();
		}

		public ISideMapButton GetSideMapButtonInstance()
		{
			return this.button;
		}

		private void ButtonOnClicked()
		{
			this.flowFactory.CreateAndPushStoryMapFlow();
			this.reminderCooldown.Reset();
		}

		private readonly IFlowFactory flowFactory;

		private readonly StoryMapEventActivation featureActivation;

		private readonly ReminderCooldown reminderCooldown;

		private readonly StoryMapSideButton button;

		private readonly Fiber fiber = new Fiber();
	}
}
