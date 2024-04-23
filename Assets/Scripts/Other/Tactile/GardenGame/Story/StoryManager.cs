using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.MapSystem;
using Tactile.GardenGame.Shop;
using Tactile.GardenGame.Story.Assets;
using TactileModules.GameCore.ButtonArea;
using TactileModules.GameCore.Inventory;
using TactileModules.GameCore.UI;
using TactileModules.PuzzleGames.GameCore;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class StoryManager : IStoryManager
	{
		public StoryManager(FlowStack flowStack, IVisualInventory visualInventory, IUIController uiController, IButtonAreaModel buttonAreaModel, UserSettingsManager userSettingsManager, IAssetModel assets, IShopViewFlowFactory shopViewFlowFactory, TimedTaskModel timedTaskModel, PropsManager propsManager)
		{
			this.flowStack = flowStack;
			this.visualInventory = visualInventory;
			this.uiController = uiController;
			this.buttonAreaModel = buttonAreaModel;
			this.userSettingsManager = userSettingsManager;
			this.assets = assets;
			this.shopViewFlowFactory = shopViewFlowFactory;
			this.timedTaskModel = timedTaskModel;
			this.propsManager = propsManager;
			this.tasks = new List<MapTask>();
			this.LastTaskInChapterComplete = new HookList<int, bool>();
			this.TaskCompleted += this.TaskCompletedHandler;
			this.StartStory();
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskCompletedByStars;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action NewTasksUnlocked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> NewTaskUnlocked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action TaskJumpedTo;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action NotEnoughStarsPlayClicked;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskStarted;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskEnded;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskSkipped;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> ChapterCompleted;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action BrowseTasksStarted;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask> TaskCompleted;



		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action ProgressionChanged;



		public StoryManager.PersistableState State
		{
			get
			{
				return UserSettingsManager.Get<StoryManager.PersistableState>();
			}
		}

		public IHookList<int, bool> LastTaskInChapterComplete { get; private set; }

		private void TaskCompletedHandler(MapTask mapTask)
		{
			this.ProgressionChanged();
		}

		private void StartStory()
		{
			if (this.IsFirstTime)
			{
				this.StartChapter(1);
			}
			else
			{
				this.LoadTasks();
				this.CheckAndApplyChanges();
			}
		}

		private bool IsFirstTime
		{
			get
			{
				return !this.State.HasTaskStates;
			}
		}

		public bool NoTasksCompleted
		{
			get
			{
				return this.tasks != null && this.tasks.Count > 0 && this.State.GetTaskState(this.tasks[0].ID) == StoryManager.PersistableState.TaskState.IntroPending && this.State.CurrentChapter == 1;
			}
		}

		public bool FirstIntroCompleted
		{
			get
			{
				if (this.tasks == null)
				{
					return false;
				}
				if (this.State.CurrentChapter > 1)
				{
					return true;
				}
				StoryManager.PersistableState.TaskState taskState = this.State.GetTaskState(this.tasks[0].ID);
				return taskState == StoryManager.PersistableState.TaskState.Active || taskState == StoryManager.PersistableState.TaskState.Completed;
			}
		}

		public bool IsFirstTaskActive
		{
			get
			{
				if (this.tasks == null)
				{
					return false;
				}
				if (this.State.CurrentChapter > 1)
				{
					return false;
				}
				StoryManager.PersistableState.TaskState taskState = this.State.GetTaskState(this.tasks[0].ID);
				return taskState == StoryManager.PersistableState.TaskState.Active;
			}
		}

		public int CurrentChapter
		{
			get
			{
				return this.State.CurrentChapter;
			}
		}

		public List<MapTask> Tasks
		{
			get
			{
				return this.tasks;
			}
		}

		public int TotalPagesCollected
		{
			get
			{
				return this.State.TotalPagesCollected;
			}
			set
			{
				this.State.TotalPagesCollected++;
				this.ProgressionChanged();
				this.userSettingsManager.SaveLocalSettings();
			}
		}

		public int GetChapterProgression()
		{
			return this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Completed).Count<MapTask>();
		}

		public bool ShouldEnterStoryMapAutomatically
		{
			get
			{
				return this.notEnoughStarsPlayClicked && this.HasStarsToCompleteATask() && this.visualInventory.InventoryManager.GetAmount("Star") > this.starAmountWhenEarnStarsClicked;
			}
		}

		public bool HasStarsToCompleteATask()
		{
			int amount = this.visualInventory.InventoryManager.GetAmount("Star");
			List<MapTask> activeTasks = this.GetActiveTasks();
			foreach (MapTask mapTask in activeTasks)
			{
				if (amount >= mapTask.StarsRequired)
				{
					return true;
				}
			}
			return false;
		}

		private void LoadTasks()
		{
			this.tasks.Clear();
			this.loadedChapter = this.State.CurrentChapter;
			string path = "Chapters/" + this.State.CurrentChapter;
			MapTask[] collection = Resources.LoadAll<MapTask>(path);
			this.tasks.AddRange(collection);
			MapTask.CalculateHierarchy(this.tasks);
		}

		private void StartChapter(int chapterNo)
		{
			this.State.CurrentChapter = chapterNo;
			this.LoadTasks();
			this.ResetTasks();
			foreach (MapTask mapTask in this.tasks)
			{
				if (mapTask.Parents.Count == 0 && mapTask.IsHierachyValid)
				{
					this.UnlockTask(mapTask, true);
				}
			}
		}

		private void ResetTasks()
		{
			foreach (MapTask mapTask in this.tasks)
			{
				this.State.SetTaskState(mapTask.ID, StoryManager.PersistableState.TaskState.Inactive);
			}
			this.State.TaskTimers.Clear();
		}

		private void UnlockTask(MapTask task, bool checkForIntro = true)
		{
			StoryManager.PersistableState.TaskState taskState = (!task.HasIntro() || !checkForIntro) ? StoryManager.PersistableState.TaskState.Active : StoryManager.PersistableState.TaskState.IntroPending;
			this.State.SetTaskState(task.ID, taskState);
			if (taskState == StoryManager.PersistableState.TaskState.Active)
			{
				this.NewTaskUnlocked(task);
			}
		}

		public void NotEnoughStarsPlayedWasClicked()
		{
			this.NotEnoughStarsPlayClicked();
			this.notEnoughStarsPlayClicked = true;
			this.starAmountWhenEarnStarsClicked = this.visualInventory.InventoryManager.GetAmount("Star");
		}

		public void ResetNotEnoughStarsClicked()
		{
			this.notEnoughStarsPlayClicked = false;
			this.starAmountWhenEarnStarsClicked = 0;
		}

		private MapTask GetTask(string id)
		{
			foreach (MapTask mapTask in this.tasks)
			{
				if (mapTask.ID == id)
				{
					return mapTask;
				}
			}
			return null;
		}

		public List<MapTask> GetActiveTasks()
		{
			return new List<MapTask>(this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Active));
		}

		public MapTask GetLastCompletedTask()
		{
			if (!string.IsNullOrEmpty(this.State.LastCompletedTask))
			{
				return this.tasks.Find((MapTask task) => task.ID == this.State.LastCompletedTask);
			}
			return null;
		}

		public void ResetLastCompletedTask()
		{
			this.State.ResetLastCompletedTask();
			this.userSettingsManager.SaveLocalSettings();
		}

		public List<MapTask> GetActiveTasks(MapAction.ActionType ofType)
		{
			return new List<MapTask>(this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Active, ofType));
		}

		public List<MapTask> GetTasks(StoryManager.PersistableState.TaskState ofState, MapAction.ActionType ofType)
		{
			return new List<MapTask>(this.IterateTasksWithState(ofState, ofType));
		}

		public MapTask GetFirstPendingIntroTask()
		{
			using (IEnumerator<MapTask> enumerator = this.IterateTasksWithState(StoryManager.PersistableState.TaskState.IntroPending).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		public float GetChapterProgressionNormalized()
		{
			int num = this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Completed).Count<MapTask>();
			return Mathf.Clamp01((float)num / (float)(this.tasks.Count - 1));
		}

		public float GetPreviousChapterProgressionNormalized()
		{
			int num = this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Completed).Count<MapTask>();
			return Mathf.Clamp01((float)(num - 1) / (float)(this.tasks.Count - 1));
		}

		public IEnumerable<MapTask> IterateTasksWithState(StoryManager.PersistableState.TaskState taskState)
		{
			StoryManager.PersistableState state = this.State;
			foreach (MapTask task in this.tasks)
			{
				if (state.GetTaskState(task.ID) == taskState)
				{
					yield return task;
				}
			}
			yield break;
		}

		public IEnumerable<MapTask> IterateTasksWithState(StoryManager.PersistableState.TaskState taskState, MapAction.ActionType ofType)
		{
			StoryManager.PersistableState state = this.State;
			foreach (MapTask task in this.tasks)
			{
				if (task.ActionType == ofType && state.GetTaskState(task.ID) == taskState)
				{
					yield return task;
				}
			}
			yield break;
		}

		public List<MapTask> AllTasks
		{
			get
			{
				return this.tasks;
			}
		}

		public IEnumerator TryCompletingTaskUsingStars(MapTask task, IEnumerator success, IEnumerator failed, IEnumerator timerStarted)
		{
			int currentStars = this.visualInventory.InventoryManager.GetAmount("Star");
			if (currentStars >= task.StarsRequired)
			{
				this.visualInventory.InventoryManager.Consume("Star", task.StarsRequired, "CompleteTask '" + task.ID + "'");
				ITimedTask timedTask = this.timedTaskModel.GetTimedTask(task);
				if (timedTask.HasTimer)
				{
					this.StartTaskTimer(task, timedTask.WaitTimeInSeconds);
					yield return success;
					yield return timerStarted;
				}
				else
				{
					yield return this.CompleteAndAnimateTask(task, success);
				}
			}
			else
			{
				yield return failed;
			}
			yield break;
		}

		public IEnumerator TryCompletingTimedTaskOrSkippingUsingCoins(MapTask task, IEnumerator success)
		{
			ITimedTask timedTask = this.timedTaskModel.GetTimedTask(task);
			if (!timedTask.HasTimer)
			{
				yield break;
			}
			int secondsLeft = this.GetSecondsRemainingInTimedTask(task);
			if (secondsLeft <= 0)
			{
				yield return this.CompleteAndAnimateTask(task, success);
				yield break;
			}
			int coins = this.visualInventory.InventoryManager.GetAmount("Coin");
			if (coins < timedTask.CoinSkipCost)
			{
				yield return this.flowStack.PushAndWait(this.shopViewFlowFactory.CreateFlow(timedTask.CoinSkipCost));
			}
			secondsLeft = this.GetSecondsRemainingInTimedTask(task);
			coins = this.visualInventory.InventoryManager.GetAmount("Coin");
			if (secondsLeft <= 0)
			{
				yield return this.CompleteAndAnimateTask(task, success);
			}
			else if (coins >= timedTask.CoinSkipCost)
			{
				this.visualInventory.InventoryManager.Consume("Coin", timedTask.CoinSkipCost, "SkipTaskTimer '" + task.Title + "'");
				yield return this.CompleteAndAnimateTask(task, success);
			}
			yield break;
		}

		private IEnumerator CompleteAndAnimateTask(MapTask task, IEnumerator successAnimation)
		{
			this.CompleteTask(task);
			yield return successAnimation;
			this.TaskCompletedByStars(task);
			yield break;
		}

		private void StartTaskTimer(MapTask task, int seconds)
		{
			DateTime time = DateTime.UtcNow.AddSeconds((double)seconds);
			this.State.SetTaskTimer(task.ID, time);
			this.userSettingsManager.SaveLocalSettings();
		}

		public bool IsTaskTimerInProgress(MapTask task)
		{
			return this.State.GetTaskState(task.ID) != StoryManager.PersistableState.TaskState.Completed && this.State.DoesTaskHaveTimer(task.ID);
		}

		public int GetSecondsRemainingInTimedTask(MapTask task)
		{
			if (!this.State.DoesTaskHaveTimer(task.ID))
			{
				return int.MaxValue;
			}
			return Mathf.Max(0, (int)(this.State.GetTaskTimer(task.ID) - DateTime.UtcNow).TotalSeconds);
		}

		public void CompleteTask(MapTask completedTask)
		{
			this.State.SetTaskState(completedTask.ID, StoryManager.PersistableState.TaskState.Completed);
			this.TaskCompleted(completedTask);
			foreach (MapTask mapTask in this.tasks)
			{
				if (this.State.GetTaskState(mapTask.ID) == StoryManager.PersistableState.TaskState.Inactive)
				{
					int num = 0;
					foreach (MapTask mapTask2 in mapTask.Parents)
					{
						if (this.State.GetTaskState(mapTask2.ID) == StoryManager.PersistableState.TaskState.Completed)
						{
							num++;
						}
					}
					if (mapTask.Parents.Count == num && num > 0)
					{
						if (!mapTask.HasIntro() && !mapTask.HasAnyDefaultActions)
						{
							this.State.SetTaskState(mapTask.ID, StoryManager.PersistableState.TaskState.Completed);
						}
						else
						{
							this.UnlockTask(mapTask, true);
						}
					}
				}
			}
			if (this.IsChapterComplete())
			{
				this.CompleteChapter();
			}
			else
			{
				this.userSettingsManager.SaveLocalSettings();
			}
			this.NewTasksUnlocked();
		}

		private bool IsChapterComplete()
		{
			StoryManager.PersistableState state = this.State;
			if (this.tasks.Count == 0)
			{
				return false;
			}
			MapTask mapTask = this.tasks[this.tasks.Count - 1];
			return state.GetTaskState(mapTask.ID) == StoryManager.PersistableState.TaskState.Completed;
		}

		public void SetTaskActive(MapTask taskToUnlock)
		{
			if (!taskToUnlock.HasAnyDefaultActions)
			{
				this.CompleteTask(taskToUnlock);
				return;
			}
			this.UnlockTask(taskToUnlock, false);
			this.NewTasksUnlocked();
			this.userSettingsManager.SaveLocalSettings();
		}

		private void CompleteChapter()
		{
			this.ChapterCompleted(this.State.CurrentChapter);
			this.StartChapter(this.State.CurrentChapter + 1);
			UserSettingsManager.Instance.SaveLocalSettings();
		}

		public void BrowseTaskStart()
		{
			this.BrowseTasksStarted();
		}

		private bool CheckForTasksReadyToActivate()
		{
			bool flag = false;
			if (this.loadedChapter != this.State.CurrentChapter)
			{
				this.LoadTasks();
				flag = true;
			}
			foreach (MapTask mapTask in this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Inactive))
			{
				bool flag2 = true;
				foreach (MapTask mapTask2 in mapTask.Parents)
				{
					if (this.State.GetTaskState(mapTask2.ID) != StoryManager.PersistableState.TaskState.Completed)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					flag = true;
					if (!mapTask.HasIntro() && !mapTask.HasAnyDefaultActions)
					{
						this.State.SetTaskState(mapTask.ID, StoryManager.PersistableState.TaskState.Completed);
					}
					else
					{
						this.UnlockTask(mapTask, true);
					}
				}
			}
			if (flag)
			{
				this.NewTasksUnlocked();
			}
			return flag;
		}

		private bool ApplyAllCompletedPropActions()
		{
			Dictionary<string, int> changedProps = new Dictionary<string, int>();
			foreach (MapTask mapTask in this.IterateAllCompletedTasks(this.State.CurrentChapter))
			{
				mapTask.IterateActionTypes<MapActionChangeProp, MapActionChoose>(delegate(MapActionChangeProp changedProp)
				{
					if (string.IsNullOrEmpty(changedProp.DecorationObjectId))
					{
						return;
					}
					changedProps[changedProp.DecorationObjectId] = changedProp.DecorationID;
				}, delegate(MapActionChoose choose)
				{
					if (changedProps.ContainsKey(choose.MapPropId))
					{
						changedProps.Remove(choose.MapPropId);
					}
				});
			}
			bool flag = false;
			foreach (KeyValuePair<string, int> keyValuePair in changedProps)
			{
				flag |= this.propsManager.SetPropSkinNoSave(keyValuePair.Key, keyValuePair.Value);
			}
			flag |= this.propsManager.TryRemoveEmptyKeys();
			return flag;
		}

		public bool CheckAndApplyChanges()
		{
			bool flag = this.CheckForTasksReadyToActivate();
			return flag | this.ApplyAllCompletedPropActions();
		}

		public bool EnsureAllUnlockedPropsArePickable(MapInformation mapInformation)
		{
			bool flag = false;
			foreach (MapTask mapTask in this.IterateAllCompletedTasks(this.State.CurrentChapter))
			{
				foreach (MapActionChoose mapActionChoose in mapTask.IterateActionTypes<MapActionChoose>())
				{
					MapProp mapComponent = mapInformation.GetMapComponent<MapProp>(mapActionChoose.MapPropId);
					if (mapComponent != null)
					{
						flag |= this.TryUnlockMapProp(mapComponent, mapActionChoose.MapPropId);
					}
				}
			}
			return flag;
		}

		private bool TryUnlockMapProp(MapProp prop, string propId)
		{
			MapProp.Variation firstPickableVariation = prop.FirstPickableVariation;
			if (firstPickableVariation == null)
			{
				return false;
			}
			int currentVariation = this.propsManager.GetPropSkin(propId);
			if (currentVariation != -1 && prop.Variations.Find((MapProp.Variation p) => p.ID == currentVariation) == null)
			{
				return this.propsManager.SetPropSkinNoSave(propId, firstPickableVariation.ID);
			}
			return !prop.VariationIdPickable(currentVariation) && this.propsManager.SetPropSkinNoSave(propId, firstPickableVariation.ID);
		}

		public IEnumerable<MapTask> IterateTasksFromChapter(int chapterNo)
		{
			string chapterPath = "Chapters/" + chapterNo.ToString();
			List<MapTask> loadedTasks = new List<MapTask>(Resources.LoadAll<MapTask>(chapterPath));
			MapTask.CalculateHierarchy(loadedTasks);
			for (int i = 0; i < loadedTasks.Count; i++)
			{
				yield return loadedTasks[i];
			}
			yield break;
		}

		private IEnumerable<MapTask> IterateTasksBeforeChapter(int chapterNo)
		{
			for (int i = 1; i < chapterNo; i++)
			{
				foreach (MapTask task in this.IterateTasksFromChapter(i))
				{
					yield return task;
				}
			}
			yield break;
		}

		private IEnumerable<MapTask> IterateAllCompletedTasks(int chapterNo)
		{
			foreach (MapTask task in this.IterateTasksBeforeChapter(chapterNo))
			{
				yield return task;
			}
			foreach (MapTask task2 in this.IterateTasksWithState(StoryManager.PersistableState.TaskState.Completed))
			{
				yield return task2;
			}
			yield break;
		}

		public void JumpToTask(int chapterNo, MapTask startingTask, MapAction.ActionType actionType, GardenGameSetup gardenGameSetup)
		{
			this.State.CurrentChapter = chapterNo;
			this.LoadTasks();
			MapTask mapTask = (!(startingTask == null)) ? this.GetTask(startingTask.ID) : ((this.tasks.Count <= 0) ? null : this.tasks[0]);
			if (mapTask == null)
			{
				if (startingTask)
				{
				}
				return;
			}
			if (startingTask == null && mapTask.HasIntro())
			{
				actionType = MapAction.ActionType.Intro;
			}
			HashSet<MapTask> tasksSet = new HashSet<MapTask>();
			mapTask.IterateHierachy(delegate(MapTask task)
			{
				tasksSet.Add(task);
				this.State.SetTaskState(task.ID, StoryManager.PersistableState.TaskState.Inactive);
			});
			this.State.SetTaskState(mapTask.ID, (!mapTask.HasIntro() || actionType != MapAction.ActionType.Intro) ? StoryManager.PersistableState.TaskState.Active : StoryManager.PersistableState.TaskState.IntroPending);
			foreach (MapTask mapTask2 in this.tasks)
			{
				if (!tasksSet.Contains(mapTask2))
				{
					this.State.SetTaskState(mapTask2.ID, StoryManager.PersistableState.TaskState.Completed);
				}
			}
			this.propsManager.Clear();
			Dictionary<string, MapProp> dictionary = new Dictionary<string, MapProp>();
			foreach (MapProp mapProp in gardenGameSetup.IterateAreaObjectsWithComponent<MapProp>())
			{
				MapObjectID component = mapProp.gameObject.GetComponent<MapObjectID>();
				if (component != null)
				{
					dictionary.Add(component.Id, mapProp);
				}
			}
			bool flag = false;
			foreach (MapTask mapTask3 in this.IterateAllCompletedTasks(this.State.CurrentChapter))
			{
				foreach (MapActionChangeProp mapActionChangeProp in mapTask3.IterateActionTypes<MapActionChangeProp>())
				{
					flag |= this.propsManager.SetPropSkinNoSave(mapActionChangeProp.DecorationObjectId, mapActionChangeProp.DecorationID);
				}
				foreach (MapActionChoose mapActionChoose in mapTask3.IterateActionTypes<MapActionChoose>())
				{
					MapProp mapProp2 = null;
					if (dictionary.TryGetValue(mapActionChoose.MapPropId, out mapProp2))
					{
						foreach (MapProp.Variation variation in mapProp2.Variations)
						{
							if (variation.IsPickable)
							{
								flag |= this.propsManager.SetPropSkinNoSave(mapActionChoose.MapPropId, variation.ID);
								break;
							}
						}
					}
				}
			}
			if (flag)
			{
				this.propsManager.Save();
			}
			this.TaskJumpedTo();
		}

		public IEnumerator RunTask(MapTask task, IStoryMapController map, MapAction.ActionType taskType, Action preEnded)
		{
			this.TaskStarted(task);
			task.TaskSkipped += this.TaskSkippedEventHandler;
			yield return new Fiber.OnExit(delegate()
			{
				if (preEnded != null)
				{
					preEnded();
				}
				this.TaskEnded(task);
				task.TaskSkipped -= this.TaskSkippedEventHandler;
			});
			yield return task.Run(map, taskType);
			yield break;
		}

		private void TaskSkippedEventHandler(MapTask mapTask)
		{
			this.TaskSkipped(mapTask);
		}

		private readonly FlowStack flowStack;

		private readonly IVisualInventory visualInventory;

		private readonly IUIController uiController;

		private readonly IButtonAreaModel buttonAreaModel;

		private readonly UserSettingsManager userSettingsManager;

		private readonly IAssetModel assets;

		private readonly IShopViewFlowFactory shopViewFlowFactory;

		private readonly TimedTaskModel timedTaskModel;

		private readonly PropsManager propsManager;

		private readonly Fiber completeChapterFiber;

		private bool notEnoughStarsPlayClicked;

		private int starAmountWhenEarnStarsClicked;

		private readonly List<MapTask> tasks;

		private int loadedChapter;

		[SettingsProvider("st", false, new Type[]
		{

		})]
		public class PersistableState : IPersistableState<StoryManager.PersistableState>, IPersistableState
		{
			private PersistableState()
			{
				this.CurrentChapter = 0;
				this.TaskStates = new Dictionary<string, int>();
				this.TaskTimers = new Dictionary<string, DateTime>();
				this.ResetLastCompletedTask();
			}

			[JsonSerializable("ch", null)]
			public int CurrentChapter { get; set; }

			[JsonSerializable("ta", typeof(int))]
			private Dictionary<string, int> TaskStates { get; set; }

			[JsonSerializable("lct", null)]
			public string LastCompletedTask { get; private set; }

			[JsonSerializable("lcr", null)]
			public float LastClaimedReward { get; private set; }

			[JsonSerializable("tt", typeof(DateTime))]
			public Dictionary<string, DateTime> TaskTimers { get; set; }

			[JsonSerializable("tp", null)]
			public int TotalPagesCollected { get; set; }

			public StoryManager.PersistableState.TaskState GetTaskState(string id)
			{
				int result;
				if (this.TaskStates.TryGetValue(id, out result))
				{
					return (StoryManager.PersistableState.TaskState)result;
				}
				return StoryManager.PersistableState.TaskState.Inactive;
			}

			public void SetTaskState(string id, StoryManager.PersistableState.TaskState newState)
			{
				this.TaskStates[id] = (int)newState;
				this.SetLastCompletedTask(id, newState);
			}

			private void SetLastCompletedTask(string id, StoryManager.PersistableState.TaskState newState)
			{
				if (newState == StoryManager.PersistableState.TaskState.Completed)
				{
					this.LastCompletedTask = id;
				}
			}

			public void SetLastClaimedReward(float lastClaimedReward)
			{
				this.LastClaimedReward = lastClaimedReward;
			}

			public void ResetLastCompletedTask()
			{
				this.LastCompletedTask = string.Empty;
			}

			public bool HasTaskStates
			{
				get
				{
					return this.TaskStates.Count > 0;
				}
			}

			public bool DoesTaskHaveTimer(string id)
			{
				return this.TaskTimers.ContainsKey(id);
			}

			public DateTime GetTaskTimer(string id)
			{
				if (this.DoesTaskHaveTimer(id))
				{
					return this.TaskTimers[id];
				}
				return DateTime.MinValue;
			}

			public void SetTaskTimer(string id, DateTime time)
			{
				this.TaskTimers[id] = time;
			}

			public void MergeFromOther(StoryManager.PersistableState newest, StoryManager.PersistableState last)
			{
				if (newest.CurrentChapter > this.CurrentChapter)
				{
					this.CurrentChapter = newest.CurrentChapter;
					this.TaskStates.Clear();
					this.TaskTimers.Clear();
					foreach (KeyValuePair<string, int> keyValuePair in newest.TaskStates)
					{
						this.TaskStates.Add(keyValuePair.Key, keyValuePair.Value);
					}
					foreach (KeyValuePair<string, DateTime> keyValuePair2 in newest.TaskTimers)
					{
						this.TaskTimers.Add(keyValuePair2.Key, keyValuePair2.Value);
					}
					return;
				}
				if (this.CurrentChapter > newest.CurrentChapter)
				{
					return;
				}
				foreach (KeyValuePair<string, int> keyValuePair3 in newest.TaskStates)
				{
					if (this.TaskStates.ContainsKey(keyValuePair3.Key))
					{
						this.TaskStates[keyValuePair3.Key] = Math.Max(this.TaskStates[keyValuePair3.Key], newest.TaskStates[keyValuePair3.Key]);
					}
					else
					{
						this.TaskStates.Add(keyValuePair3.Key, keyValuePair3.Value);
					}
				}
				foreach (KeyValuePair<string, DateTime> keyValuePair4 in newest.TaskTimers)
				{
					if (this.TaskTimers.ContainsKey(keyValuePair4.Key))
					{
						bool flag = DateTime.Compare(newest.TaskTimers[keyValuePair4.Key], this.TaskTimers[keyValuePair4.Key]) < 0;
						if (flag)
						{
							this.TaskTimers[keyValuePair4.Key] = newest.TaskTimers[keyValuePair4.Key];
						}
					}
					else
					{
						this.TaskTimers.Add(keyValuePair4.Key, keyValuePair4.Value);
					}
				}
			}

			public enum TaskState
			{
				Inactive,
				IntroPending,
				Active,
				Completed
			}
		}
	}
}
