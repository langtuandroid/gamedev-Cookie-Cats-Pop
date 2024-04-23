using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.Story;
using TactileModules.GameCore.Boot;
using TactileModules.GameCore.UI;
using TactileModules.GardenGame.MapSystem.Assets;
using TactileModules.Placements;
using TactileModules.PuzzleGames.GameCore;
using TactileModules.SagaCore;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MainMapState : INotifiedFlow, INextMapDot, IFullScreenOwner, MapPopupManager.IPopupManagerProvider, IFlow, IFiberRunnable
	{
		public MainMapState(IUIController uiController, IFullScreenManager fullScreenManager, IStoryControllerFactory storyControllerFactory, StoryManager storyManager, PropsManager propsManager, IAssetModel assets, IPlacementRunner placementRunner, IUserSettings userSettings, FlowStack flowStack, IMainMapStateProvider mainMapStateProvider)
		{
			this.uiController = uiController;
			this.fullScreenManager = fullScreenManager;
			this.storyControllerFactory = storyControllerFactory;
			this.storyManager = storyManager;
			this.propsManager = propsManager;
			this.assets = assets;
			this.placementRunner = placementRunner;
			this.mainMapStateProvider = mainMapStateProvider;
			this.StartSequenceHooks = new BreakableHookList<MainMapState>();
			this.mainMapController = new MainMapController(uiController, propsManager, assets, flowStack, userSettings);
			this.startSequenceRunner = new SignaledSequenceRunner(new Func<IEnumerator>(this.StartSequence), new Func<bool>(this.IsMapIdle));
			this.resumeFromPlayRunner = new SignaledSequenceRunner(new Func<IEnumerator>(this.ResumeFromLevelPlaySequence), new Func<bool>(this.IsMapIdle));
		}

		public BreakableHookList<MainMapState> StartSequenceHooks { get; private set; }

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action MainScreenAcquired;

		public void StartTask(MapTask task, MapAction.ActionType taskType)
		{
			this.testTask = task;
			this.testTaskType = taskType;
		}

		IEnumerator IFullScreenOwner.ScreenAcquired()
		{
			this.mainMapController.ScreenAcquired();
			this.storyController = this.storyControllerFactory.CreateController(this.mainMapController);
			this.BuildHandleTaskUnlockedRunner();
			if (this.MainScreenAcquired != null)
			{
				this.MainScreenAcquired();
			}
			if (this.testTask == null)
			{
				yield return this.storyController.TryPlayEnterMapOrPendingIntro();
			}
			yield break;
		}

		private void BuildHandleTaskUnlockedRunner()
		{
			this.handleTaskUnlockedRunner = new SignaledSequenceRunner(new Func<IEnumerator>(this.storyController.TryStartAndWaitForPendingIntro), new Func<bool>(this.IsMapIdle));
			this.storyManager.NewTasksUnlocked += this.handleTaskUnlockedRunner.SetSignal;
		}

		private IEnumerator StartSequence()
		{
			this.startSequenceHasRun = true;
			EnumeratorResult<bool> wasFlowBroken = new EnumeratorResult<bool>();
			yield return this.StartSequenceHooks.InvokeAll(this, wasFlowBroken);
			if (wasFlowBroken)
			{
				yield break;
			}
			yield return this.placementRunner.Run(PlacementIdentifier.MapIdle, wasFlowBroken);
			yield break;
		}

		private IEnumerator ResumeFromLevelPlaySequence()
		{
			EnumeratorResult<bool> wasFlowBroken = new EnumeratorResult<bool>();
			yield return this.placementRunner.Run(PlacementIdentifier.MapIdle, wasFlowBroken);
			yield break;
		}

		void IFullScreenOwner.ScreenReady()
		{
			this.screenIsReady = true;
			if (this.startSequenceHasRun)
			{
				this.resumeFromPlayRunner.SetSignal();
			}
			else
			{
				this.startSequenceRunner.SetSignal();
			}
		}

		void IFullScreenOwner.ScreenLost()
		{
			this.screenIsReady = false;
			this.storyController.Destroy();
			this.mainMapController.ScreenLost();
		}

		void INotifiedFlow.Enter(IFlow previousFlow)
		{
			this.flowHasFocus = true;
		}

		void INotifiedFlow.Leave(IFlow nextFlow)
		{
			this.flowHasFocus = false;
		}

		private bool IsMapIdle()
		{
			return this.screenIsReady && this.flowHasFocus;
		}

		public IEnumerator Run()
		{
			if (this.testTask != null)
			{
				this.fullScreenManager.PushInstantly(this);
				this.storyController.StartTask(this.testTask, this.testTaskType);
				while (this.storyController.Step())
				{
					this.mainMapController.Step(Time.deltaTime);
					yield return null;
				}
				this.testTask = null;
			}
			else
			{
				yield return this.fullScreenManager.Push(this);
			}
			this.isRunning = true;
			while (this.isRunning)
			{
				this.mainMapController.Step(Time.deltaTime);
				yield return this.handleTaskUnlockedRunner.TryRun();
				yield return this.startSequenceRunner.TryRun();
				yield return this.resumeFromPlayRunner.TryRun();
				this.storyController.Step();
				yield return null;
				if (!this.storyController.IsStoryInProgress() && !this.storyController.IsIdleBehaviourRunning())
				{
					List<MapTask> tasks = this.storyManager.GetTasks(StoryManager.PersistableState.TaskState.Completed, MapAction.ActionType.Idle);
					if (tasks.Count > 0)
					{
						this.storyController.StartIdleBehaviour(tasks[tasks.Count - 1]);
					}
				}
			}
			yield return this.fullScreenManager.Pop();
			yield break;
		}

		public void EndFlow()
		{
			this.isRunning = false;
		}

		public void OnExit()
		{
		}

		IEnumerator MapPopupManager.IPopupManagerProvider.AnimateProgress()
		{
			yield break;
		}

		MapPopupManager.PopupConfig MapPopupManager.IPopupManagerProvider.PopupConfig
		{
			get
			{
				return ConfigurationManager.Get<MapPopupManager.PopupConfig>();
			}
		}

		public int NextDotIndexToOpen
		{
			get
			{
				return this.mainMapStateProvider.GetNextDotIndexToOpen();
			}
		}

		private readonly IUIController uiController;

		private readonly IFullScreenManager fullScreenManager;

		private readonly IStoryControllerFactory storyControllerFactory;

		private readonly PropsManager propsManager;

		private readonly StoryManager storyManager;

		private readonly IAssetModel assets;

		private readonly IPlacementRunner placementRunner;

		private readonly IMainMapStateProvider mainMapStateProvider;

		private MainMapController mainMapController;

		private IStoryController storyController;

		private bool isRunning;

		private MapTask testTask;

		private MapAction.ActionType testTaskType;

		private SignaledSequenceRunner startSequenceRunner;

		private SignaledSequenceRunner resumeFromPlayRunner;

		private SignaledSequenceRunner handleTaskUnlockedRunner;

		private bool startSequenceHasRun;

		private bool flowHasFocus;

		private bool screenIsReady;
	}
}
