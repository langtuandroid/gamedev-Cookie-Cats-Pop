using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Fibers;
using Tactile.GardenGame.MapSystem;
using TactileModules.Validation;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
    public class MapTask : MonoBehaviour
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapAction> ActionStarted;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapAction, object> ActionEnded;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapTask> TaskSkipped;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public event Action<MapTask, MapAction, bool> TaskSkippedToAction;

        public MapAction.ActionType ActionType
        {
            get
            {
                return this.actionType;
            }
            set
            {
                this.actionType = value;
            }
        }

        public MapAction StartAction { get; set; }

        public MapAction CurrentAction { get; set; }

        public IEnumerator Run(IStoryMapController map, MapAction.ActionType taskType)
        {
            List<MapAction> actions = new List<MapAction>();
            this.GetActions(actions, taskType);
            this.skipped = 0;
            bool isSkipping = false;
            bool checkIsSkippingToAction = false;
            Action onSkipClicked = delegate ()
            {
                this.skipped++;
                isSkipping = true;
                checkIsSkippingToAction = true;
                if (this.TaskSkipped != null)
                {
                    this.TaskSkipped(this);
                }
            };
            map.Dialog.SkipClicked += onSkipClicked;
            FiberRunner dontWaitTasks = new FiberRunner(FiberBucket.Manual);
            Fiber normalTasks = new Fiber(FiberBucket.Manual);
            yield return new Fiber.OnExit(delegate ()
            {
                map.Dialog.SkipClicked -= onSkipClicked;
                dontWaitTasks.Terminate();
                normalTasks.Terminate();
                this.CurrentAction = null;
            });
            foreach (MapAction action in actions)
            {
                this.CurrentAction = action;
                if (!isSkipping || action.IsAllowedWhenSkipping)
                {
                    if (action.DontWait)
                    {
                        dontWaitTasks.Run(this.RunAction(action, map), false);
                    }
                    else
                    {
                        normalTasks.Start(this.RunAction(action, map));
                    }
                    if (this.ShouldMapActionResetSkipping(action))
                    {
                        isSkipping = false;
                    }
                    if (checkIsSkippingToAction && !isSkipping)
                    {
                        checkIsSkippingToAction = false;
                        this.TaskSkippedTo(action, false);
                    }
                    if (!isSkipping)
                    {
                        for (; ; )
                        {
                            dontWaitTasks.Step();
                            if (!normalTasks.Step())
                            {
                                break;
                            }
                            yield return null;
                        }
                    }
                    else
                    {
                        dontWaitTasks.Step();
                        normalTasks.Step();
                        dontWaitTasks.Terminate();
                        normalTasks.Terminate();
                    }
                }
            }
            if (isSkipping)
            {
                this.TaskSkippedTo(actions[actions.Count - 1], true);
            }
            dontWaitTasks.Terminate();
            this.StopAllFollowers(map);
            yield return map.Dialog.CloseDialog();
            yield return map.HideFullScreenImage();
            yield break;
        }

        private bool ShouldMapActionResetSkipping(MapAction action)
        {
            return action is MapActionChoose;
        }

        private void TaskSkippedTo(MapAction action, bool isLastAcion)
        {
            if (this.TaskSkippedToAction != null)
            {
                this.TaskSkippedToAction(this, action, isLastAcion);
            }
        }

        private IEnumerator RunAction(MapAction action, IStoryMapController map)
        {
            if (this.ActionStarted != null)
            {
                this.ActionStarted(action);
            }
            IMapActionResult actionResult = action as IMapActionResult;
            yield return action.Logic(map);
            if (this.ActionEnded != null)
            {
                this.ActionEnded(action, (actionResult == null) ? null : actionResult.GetResult());
            }
            yield break;
        }

        private void StartFromAction(List<MapAction> actions, IStoryMapController map, MapAction targetAction)
        {
            if (!actions.Contains(targetAction))
            {
                return;
            }
            Fiber fiber = new Fiber(FiberBucket.Manual);
            for (int i = 0; i < actions.Count; i++)
            {
                MapAction mapAction = actions[i];
                if (mapAction == targetAction)
                {
                    break;
                }
                bool flag = mapAction is MapActionCloseDialog || mapAction is MapActionDialog;
                MapActionChoose mapActionChoose = mapAction as MapActionChoose;
                if (mapActionChoose != null)
                {
                    mapActionChoose.ChooseAutomatically(map);
                }
                else if (!flag)
                {
                    fiber.Start(this.RunAction(mapAction, map));
                    if (mapAction.IsAllowedWhenSkipping)
                    {
                        fiber.Step();
                        fiber.Terminate();
                    }
                    else
                    {
                        while (fiber.Step())
                        {
                        }
                    }
                }
                actions.RemoveAt(i);
                i--;
            }
        }

        private void StopAllFollowers(IStoryMapController map)
        {
            foreach (MapMoveable mapMoveable in map.IterateObjectsWithComponent<MapMoveable>())
            {
                mapMoveable.StopFollow();
            }
        }

        public void GetActions(List<MapAction> actions, MapAction.ActionType actionType)
        {
            actions.Clear();
            actions.AddRange(base.GetComponents<MapAction>());
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i].Type != actionType)
                {
                    actions.RemoveAt(i);
                    i--;
                }
            }
            actions.Sort((MapAction a, MapAction b) => a.Position.y.CompareTo(b.Position.y));
        }

        public List<T> GetActionsOfType<T>(MapAction.ActionType actionType) where T : MapAction
        {
            List<MapAction> list = new List<MapAction>();
            this.GetActions(list, actionType);
            List<T> list2 = new List<T>();
            foreach (MapAction mapAction in list)
            {
                if (mapAction is T)
                {
                    list2.Add(mapAction as T);
                }
            }
            return list2;
        }

        public bool HasAnyDefaultActions
        {
            get
            {
                List<MapAction> list = new List<MapAction>();
                this.GetActions(list, MapAction.ActionType.Default);
                return list.Count > 0;
            }
        }

        public bool HasIntro()
        {
            MapTask.internalGetComponentList.Clear();
            base.GetComponents<MapAction>(MapTask.internalGetComponentList);
            if (this.ActionType == MapAction.ActionType.Intro)
            {
                return true;
            }
            for (int i = 0; i < MapTask.internalGetComponentList.Count; i++)
            {
                if (MapTask.internalGetComponentList[i].Type == MapAction.ActionType.Intro)
                {
                    return true;
                }
            }
            return false;
        }

        public int X
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
            set
            {
                this.y = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public int Left
        {
            get
            {
                return this.x;
            }
        }

        public int Right
        {
            get
            {
                return this.x + this.width;
            }
        }

        public List<MapTask> Parents { get; private set; }

        public List<MapTask> Children { get; private set; }

        public bool IsHierachyValid { get; private set; }

        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public int StarsRequired
        {
            get
            {
                return this.starsRequired;
            }
            set
            {
                this.starsRequired = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
            }
        }

        public bool IsBuildTask
        {
            get
            {
                return this.isBuildTask;
            }
            set
            {
                this.isBuildTask = value;
            }
        }

        public bool SkippedTask
        {
            get
            {
                return this.skipped > 0;
            }
        }

        private bool Overlap(MapTask other)
        {
            return this.Right > other.Left && this.Left < other.Right;
        }

        private static void CalculateRelationshipsForTask(List<MapTask> tasks, MapTask task)
        {
            List<MapTask> list = new List<MapTask>(MapTask.IterateTasksAbove(tasks, task));
            List<MapTask> list2 = new List<MapTask>(list);
            foreach (MapTask task2 in list2)
            {
                foreach (MapTask item in MapTask.IterateTasksAbove(tasks, task2))
                {
                    list.Remove(item);
                }
            }
            foreach (MapTask mapTask in list)
            {
                task.Parents.Add(mapTask);
                mapTask.Children.Add(task);
            }
        }

        public static IEnumerable<MapTask> IterateTasksAbove(List<MapTask> tasks, MapTask task)
        {
            foreach (MapTask otherTask in tasks)
            {
                if (otherTask.y < task.y && task.Overlap(otherTask))
                {
                    yield return otherTask;
                }
            }
            yield break;
        }

        public static void CalculateHierarchy(List<MapTask> tasks)
        {
            MapTask.SortTasks(tasks);
            MapTask.CreateParentAndChildrenLists(tasks);
            MapTask.CalculateRelationships(tasks);
            MapTask.ValidateParentReferences(tasks);
        }

        private static void SortTasks(List<MapTask> tasks)
        {
            tasks.Sort(delegate (MapTask a, MapTask b)
            {
                if (a.y < b.y)
                {
                    return -1;
                }
                if (a.y > b.y)
                {
                    return 1;
                }
                if (a.x < b.x)
                {
                    return -1;
                }
                if (a.x > b.x)
                {
                    return 1;
                }
                return 0;
            });
        }

        private static void CreateParentAndChildrenLists(List<MapTask> tasks)
        {
            foreach (MapTask mapTask in tasks)
            {
                mapTask.Parents = new List<MapTask>();
                mapTask.Children = new List<MapTask>();
            }
        }

        private static void CalculateRelationships(List<MapTask> sortedTasks)
        {
            foreach (MapTask task in sortedTasks)
            {
                MapTask.CalculateRelationshipsForTask(sortedTasks, task);
            }
        }

        private static void ValidateParentReferences(List<MapTask> sortedTasks)
        {
            int num = 0;
            foreach (MapTask mapTask in sortedTasks)
            {
                if (mapTask.Parents.Count == 0)
                {
                    mapTask.IsHierachyValid = (num == 0);
                }
                else
                {
                    mapTask.IsHierachyValid = true;
                }
            }
        }

        public void IterateHierachy(Action<MapTask> onChild)
        {
            onChild(this);
            for (int i = 0; i < this.Children.Count; i++)
            {
                this.Children[i].IterateHierachy(onChild);
            }
        }

        public IEnumerable<T> IterateActionTypes<T>() where T : MapAction
        {
            T[] actionsOfType = base.GetComponents<T>();
            for (int i = 0; i < actionsOfType.Length; i++)
            {
                yield return actionsOfType[i];
            }
            yield break;
        }

        public void IterateActionTypes<T, T2>(Action<T> onType1, Action<T2> onType2) where T : MapAction where T2 : MapAction
        {
            List<MapAction> actions = new List<MapAction>();
            this.GetActions(actions, MapAction.ActionType.Intro);
            this.IterateActionTypes<T, T2>(actions, onType1, onType2);
            this.GetActions(actions, MapAction.ActionType.Default);
            this.IterateActionTypes<T, T2>(actions, onType1, onType2);
        }

        private void IterateActionTypes<T, T2>(List<MapAction> actions, Action<T> onType1, Action<T2> onType2) where T : MapAction where T2 : MapAction
        {
            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is T)
                {
                    onType1((T)((object)actions[i]));
                }
                else if (actions[i] is T2)
                {
                    onType2((T2)((object)actions[i]));
                }
            }
        }

        [SerializeField]
        private int x;

        [SerializeField]
        private int y;

        [SerializeField]
        private int width = 4;

        [SerializeField]
        private int starsRequired = 1;

        [SerializeField]
        private string title;

        [SerializeField]
        [OptionalSerializedField]
        private string timedTaskDescription;

        [SerializeField]
        private Texture2D icon;

        [SerializeField]
        private string id;

        [SerializeField]
        private bool isBuildTask;

        [SerializeField]
        private MapAction.ActionType actionType;

        private int skipped;

        private static List<MapAction> internalGetComponentList = new List<MapAction>();
    }
}
