using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.Story;
using TactileModules.Placements;

namespace TactileModules.PuzzleGames.StoryMapEvent
{
	public class PlacementRunnable : IPlacementRunnableCanBreak, IPlacementRunnable
	{
		public PlacementRunnable(IFlowFactory flowFactory, IStoryMapEventActivation featureActivation, IViewFactory viewFactory, IReminderCooldown reminderCooldown, IStoryManager storyManager)
		{
			this.flowFactory = flowFactory;
			this.featureActivation = featureActivation;
			this.viewFactory = viewFactory;
			this.reminderCooldown = reminderCooldown;
			this.storyManager = storyManager;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<int> TimeLeftChanged = delegate (int A_0)
        {
        };



        public string ID
		{
			get
			{
				return "StoryMapEventPopups";
			}
		}

		public IEnumerator Run(IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow)
		{
			if (this.fiber.IsTerminated)
			{
				this.fiber.Start(this.UpdateTimeLeft());
			}
			if (this.featureActivation.ShouldActivateStoryMap())
			{
				yield return this.RunFeatureStarted(placementViewMediator, breakFlow);
			}
			else if (this.featureActivation.ShouldDeactivateStoryMap())
			{
				yield return this.RunFeatureEnded(placementViewMediator, breakFlow);
			}
			else if (this.storyManager.ShouldEnterStoryMapAutomatically)
			{
				breakFlow.value = true;
				this.StartStoryMapFlow();
			}
			else if (this.featureActivation.HasActiveFeature() && this.reminderCooldown.IsTimeToShow())
			{
				yield return this.RunFeatureReminder(placementViewMediator, breakFlow);
			}
			yield break;
		}

		private IEnumerator RunFeatureStarted(IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow)
		{
			this.featureActivation.ActivateStoryMap();
			IStoryMapEventAnnounceView view = this.viewFactory.CreateFeatureStartedView();
			yield return this.ShowView(view, placementViewMediator, breakFlow);
			yield break;
		}

		private IEnumerator RunFeatureEnded(IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow)
		{
			this.featureActivation.DeactivateStoryMap();
			IStoryMapEventAnnounceView view = this.viewFactory.CreateFeatureEndedView();
			yield return this.ShowAnnounceView(view, placementViewMediator, delegate
			{
				breakFlow.value = false;
			});
			yield break;
		}

		private IEnumerator RunFeatureReminder(IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow)
		{
			IStoryMapEventAnnounceView view = this.viewFactory.CreateFeatureReminderView();
			yield return this.ShowView(view, placementViewMediator, breakFlow);
			yield break;
		}

		private IEnumerator ShowView(IStoryMapEventAnnounceView view, IPlacementViewMediator placementViewMediator, EnumeratorResult<bool> breakFlow)
		{
			this.reminderCooldown.Reset();
			yield return this.ShowAnnounceView(view, placementViewMediator, delegate
			{
				breakFlow.value = true;
				this.StartStoryMapFlow();
			});
			yield break;
		}

		private IEnumerator ShowAnnounceView(IStoryMapEventAnnounceView view, IPlacementViewMediator placementViewMediator, Action callToAction)
		{
			bool hasClickedCallToAction = false;
			bool didClose = false;
			view.DismissClicked += delegate()
			{
				view.Close(0);
				didClose = true;
			};
			view.CallToActionClicked += delegate()
			{
				hasClickedCallToAction = true;
				view.Close(0);
				didClose = true;
			};
			this.TimeLeftChanged += view.SetTimeLeft;
			placementViewMediator.ShowViewInstance<IStoryMapEventAnnounceView>(view, new object[0]);
			while (!didClose)
			{
				yield return null;
			}
			if (hasClickedCallToAction)
			{
				callToAction();
			}
			yield break;
		}

		private void StartStoryMapFlow()
		{
			this.flowFactory.CreateAndPushStoryMapFlow();
		}

		private IEnumerator UpdateTimeLeft()
		{
			int lastSecondsLeft = int.MaxValue;
			for (;;)
			{
				int secondsLeft = this.featureActivation.GetSecondsLeft();
				if (lastSecondsLeft != secondsLeft)
				{
					this.TimeLeftChanged(secondsLeft);
					lastSecondsLeft = secondsLeft;
				}
				yield return null;
			}
			yield break;
		}

		private readonly IFlowFactory flowFactory;

		private readonly IStoryMapEventActivation featureActivation;

		private readonly IViewFactory viewFactory;

		private readonly IReminderCooldown reminderCooldown;

		private readonly IStoryManager storyManager;

		private readonly Fiber fiber = new Fiber();
	}
}
