using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.MapSystem;

namespace Tactile.GardenGame.Story
{
	public class StoryController : IStoryController
	{
		public StoryController(StoryManager storyManager, IStoryMapControllerFactory storyMapControllerFactory, PropsManager propsManager, MainMapController mainMapController)
		{
			this.storyManager = storyManager;
			this.storyMapControllerFactory = storyMapControllerFactory;
			this.mainMapController = mainMapController;
			this.logicFiber = new Fiber(FiberBucket.Manual);
			this.idleBehaviourFiber = new Fiber(FiberBucket.Manual);
			if (this.storyManager != null)
			{
				this.storyManager.TaskCompletedByStars += this.StoryOnTaskCompletedByStars;
				this.storyManager.TaskJumpedTo += mainMapController.Map.ApplySavedState;
			}
			this.choosePropPersistHandler = new ChoosePropPersistHandler(propsManager, storyManager, mainMapController.Map.Entities);
			if (this.storyManager.EnsureAllUnlockedPropsArePickable(mainMapController.Map.Entities))
			{
				propsManager.Save();
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskStarted;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskEnded;

		public IEnumerator TryPlayEnterMapOrPendingIntro()
		{
			if (!this.logicFiber.IsTerminated)
			{
				yield break;
			}
			this.storyManager.ResetNotEnoughStarsClicked();
			if (this.StartPendingIntro())
			{
				while (this.logicFiber.Step())
				{
					yield return null;
				}
			}
			else if (this.StartEnterMapTask())
			{
				while (this.logicFiber.Step())
				{
					yield return null;
				}
			}
			yield break;
		}

		public IEnumerator TryStartAndWaitForPendingIntro()
		{
			if (!this.logicFiber.IsTerminated)
			{
				yield break;
			}
			if (this.StartPendingIntro())
			{
				while (this.logicFiber.Step())
				{
					yield return null;
				}
			}
			yield break;
		}

		public void Destroy()
		{
			this.storyManager.TaskCompletedByStars -= this.StoryOnTaskCompletedByStars;
			this.storyManager.TaskJumpedTo -= this.mainMapController.Map.ApplySavedState;
			this.logicFiber.Terminate();
			this.choosePropPersistHandler.Destroy();
		}

		public bool Step()
		{
			this.idleBehaviourFiber.Step();
			return this.logicFiber.Step();
		}

		public bool IsStoryInProgress()
		{
			return !this.logicFiber.IsTerminated;
		}

		private bool StartEnterMapTask()
		{
			this.StopIdleBehaviour();
			List<MapTask> tasks = this.storyManager.GetTasks(StoryManager.PersistableState.TaskState.Completed, MapAction.ActionType.EnterMap);
			if (tasks.Count > 0)
			{
				this.logicFiber.Start(this.PlayTask(tasks[tasks.Count - 1], MapAction.ActionType.EnterMap, null));
				return true;
			}
			return false;
		}

		private bool StartPendingIntro()
		{
			this.StopIdleBehaviour();
			MapTask introTask = this.storyManager.GetFirstPendingIntroTask();
			if (introTask != null)
			{
				this.logicFiber.Start(this.PlayTask(introTask, MapAction.ActionType.Intro, delegate
				{
					this.storyManager.SetTaskActive(introTask);
				}));
				return true;
			}
			return false;
		}

		private void StoryOnTaskCompletedByStars(MapTask task)
		{
			this.StopIdleBehaviour();
			this.logicFiber.Start(this.PlayTask(task, MapAction.ActionType.Default, delegate
			{
				this.StartPendingIntro();
			}));
		}

		public bool IsIdleBehaviourRunning()
		{
			return !this.idleBehaviourFiber.IsTerminated;
		}

		public void StartIdleBehaviour(MapTask task)
		{
			if (!this.idleBehaviourFiber.IsTerminated)
			{
				return;
			}
			this.idleBehaviourFiber.Start(this.PlayIdleTask(task));
		}

		public void StopIdleBehaviour()
		{
			if (!this.idleBehaviourFiber.IsTerminated)
			{
				this.idleBehaviourFiber.Terminate();
			}
		}

		private IEnumerator PlayIdleTask(MapTask task)
		{
			IStoryMapController storyMapController = this.storyMapControllerFactory.Create(this.mainMapController);
			this.mainMapController.Map.InteractionEnabled = true;
			storyMapController.Camera.LimitsEnabled = true;
			yield return new Fiber.OnTerminate(delegate()
			{
				storyMapController.Destroy();
				storyMapController = null;
			});
			yield return task.Run(storyMapController, MapAction.ActionType.Idle);
			if (storyMapController != null)
			{
				storyMapController.Destroy();
			}
			yield return null;
			yield break;
		}

		public IEnumerator PlayTask(MapTask task, MapAction.ActionType taskType, Action onComplete = null)
		{
			if (this.TaskStarted != null)
			{
				this.TaskStarted(task);
			}
			IStoryMapController storyMapController = this.storyMapControllerFactory.Create(this.mainMapController);
			yield return new Fiber.OnExit(delegate()
			{
				storyMapController.Destroy();
				this.mainMapController.Map.InteractionEnabled = true;
				storyMapController.Camera.LimitsEnabled = true;
				if (this.TaskEnded != null)
				{
					this.TaskEnded(task);
				}
			});
			int currentChapter = this.storyManager.CurrentChapter;
			bool isLastTaskInChapter = this.storyManager.Tasks.Count > 0 && this.storyManager.Tasks.Last<MapTask>().ID == task.ID;
			storyMapController.Camera.LimitsEnabled = false;
			this.mainMapController.Map.InteractionEnabled = false;
			yield return this.mainMapController.UI.HideUI();
			yield return this.storyManager.RunTask(task, storyMapController, taskType, onComplete);
			yield return this.mainMapController.UI.ShowUI();
			if (isLastTaskInChapter)
			{
				bool isEndOfStoryContent = this.storyManager.GetActiveTasks().Count == 0;
				yield return this.storyManager.LastTaskInChapterComplete.InvokeAll(currentChapter, isEndOfStoryContent);
			}
			yield break;
		}

		public void StartTask(MapTask task, MapAction.ActionType taskType)
		{
			if (taskType == MapAction.ActionType.Intro)
			{
				this.StartPendingIntro();
				return;
			}
			this.storyManager.CompleteTask(task);
			this.StoryOnTaskCompletedByStars(task);
		}

		private readonly StoryManager storyManager;

		private readonly IStoryMapControllerFactory storyMapControllerFactory;

		private readonly MainMapController mainMapController;

		private readonly Fiber logicFiber;

		private readonly ChoosePropPersistHandler choosePropPersistHandler;

		private readonly Fiber idleBehaviourFiber;
	}
}
