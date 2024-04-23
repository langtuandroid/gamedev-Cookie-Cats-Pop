using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Tactile.GardenGame.Story.Rewards;
using TactileModules.GameCore.Inventory;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class BrowseTasksView : ExtensibleView<IBrowseTaskViewExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MapTask, Vector3> TaskClicked;



		public bool LockedCloseInput { get; set; }

		public void Initialize(IStoryManager storyManager, TimedTaskModel timedTaskModel)
		{
			this.storyManager = storyManager;
			this.timedTaskModel = timedTaskModel;
			this.Refresh();
		}

		private void RefreshChapter(bool showPreviousProgression)
		{
			this.chapterText.text = string.Format(L.Get("Day {0}"), this.storyManager.CurrentChapter);
			this.chapterNumber.text = this.storyManager.CurrentChapter.ToString();
			this.SetProgress((!showPreviousProgression) ? this.storyManager.GetChapterProgressionNormalized() : this.storyManager.GetPreviousChapterProgressionNormalized());
		}

		public void SetProgress(float progress)
		{
			this.progressFill.FillAmount = progress;
			this.progressBarLabel.text = progress.ToString("P0");
		}

		public void Refresh()
		{
			List<MapTask> activeTasks = this.storyManager.GetActiveTasks(MapAction.ActionType.Default);
			MapTask mapTask = this.storyManager.GetLastCompletedTask();
			bool flag = mapTask != null;
			if (flag && mapTask.StarsRequired < 1)
			{
				mapTask = null;
			}
			this.RefreshChapter(flag);
			this.RefreshTasks(activeTasks, mapTask);
		}

		private void RefreshTasks(List<MapTask> tasks, MapTask lastCompletedTask)
		{
			IEnumerator enumerator = this.taskArea.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.outOfContent.gameObject.SetActive(tasks.Count == 0);
			if (lastCompletedTask != null)
			{
				TaskCompletedVisuals taskCompletedVisuals = UnityEngine.Object.Instantiate<TaskCompletedVisuals>(this.taskCompletedVisualsPrefab);
				taskCompletedVisuals.Initialize(lastCompletedTask);
				this.HandleTaskWidgetPlacement(taskCompletedVisuals);
			}
			foreach (MapTask mapTask in tasks)
			{
				TaskCompletableButton taskCompletableButton = UnityEngine.Object.Instantiate<TaskCompletableButton>(this.taskCompletableButtonPrefab);
				taskCompletableButton.Initialize(mapTask, this.timedTaskModel.GetTimedTask(mapTask));
				this.HandleTaskWidgetPlacement(taskCompletableButton);
				taskCompletableButton.OnButtonClicked += this.TaskButtonClicked;
			}
		}

		private void HandleTaskWidgetPlacement(MonoBehaviour widget)
		{
			widget.gameObject.SetLayerRecursively(base.gameObject.layer);
			widget.transform.SetParent(this.taskArea.transform, false);
			widget.transform.localPosition = Vector3.zero;
		}

		private void TaskButtonClicked(TaskCompletableButton taskCompletableButton)
		{
			this.TaskClicked(taskCompletableButton.Task, taskCompletableButton.StarTargetTransform.position);
		}

		public void AddRewardIndicator(StoryRewardIndicator rewardItem, float progression)
		{
			if (base.Extension != null)
			{
				base.Extension.AddRewardIndicator(rewardItem, progression);
			}
		}

		public IEnumerator AnimateTaskCompleted()
		{
			if (base.Extension != null)
			{
				MapTask lastCompletedTask = this.storyManager.GetLastCompletedTask();
				if (lastCompletedTask != null)
				{
					yield return base.Extension.AnimateTaskCompleted(this.storyManager);
				}
			}
			else
			{
				this.SetProgress(this.storyManager.GetChapterProgressionNormalized());
			}
			yield break;
		}

		private void CloseButtonClicked(UIEvent e)
		{
			if (!this.LockedCloseInput)
			{
				base.Close(0);
			}
		}

		private void CheatButtonClicked(UIEvent e)
		{
		}

		public IFlyingItemsAnimator TaskStarAnimator
		{
			get
			{
				return (base.Extension == null) ? null : base.Extension.TaskStarAnimator;
			}
		}

		private Action<MapTask> registeredCompleteTaskClicked;

		private Action registeredCheatClicked;

		[SerializeField]
		private UIFillModifier progressFill;

		[SerializeField]
		private UILabel progressBarLabel;

		[SerializeField]
		private UILayout taskArea;

		[SerializeField]
		private TaskCompletedVisuals taskCompletedVisualsPrefab;

		[SerializeField]
		private TaskCompletableButton taskCompletableButtonPrefab;

		[SerializeField]
		private UILabel chapterText;

		[SerializeField]
		private UILabel chapterNumber;

		[SerializeField]
		private UILabel outOfContent;

		private IStoryManager storyManager;

		private TimedTaskModel timedTaskModel;
	}
}
