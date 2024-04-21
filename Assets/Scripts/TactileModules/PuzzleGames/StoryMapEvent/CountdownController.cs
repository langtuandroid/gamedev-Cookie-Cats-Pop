using System;
using System.Collections;
using Fibers;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class CountdownController
	{
		public CountdownController(StoryMapEventActivation featureActivation, StoryMapEventCountdown component)
		{
			this.featureActivation = featureActivation;
			this.component = component;
			this.fiber.Start(this.UpdateCountdown());
		}

		private IEnumerator UpdateCountdown()
		{
			int lastSecondsLeft = int.MaxValue;
			for (;;)
			{
				int secondsLeft = this.featureActivation.GetSecondsLeft();
				if (lastSecondsLeft != secondsLeft)
				{
					this.component.SetCountdown(secondsLeft);
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

		private readonly StoryMapEventActivation featureActivation;

		private readonly StoryMapEventCountdown component;

		private readonly Fiber fiber = new Fiber();
	}
}
