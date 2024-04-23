using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class TasksCheatView : UIView
	{
		private void Dismiss(UIEvent e)
		{
			base.Close(0);
		}

		public void RefreshActiveTasks(List<MapTask> tasks)
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
			int num = 0;
			foreach (MapTask mapTask in tasks)
			{
				if (mapTask.ActionType == MapAction.ActionType.Default)
				{
					TaskCompletableButton taskCompletableButton = UnityEngine.Object.Instantiate<TaskCompletableButton>(this.taskCompletableButtonPrefab);
					taskCompletableButton.Initialize(mapTask, new CheatViewTimedTaskData());
					taskCompletableButton.gameObject.SetLayerRecursively(base.gameObject.layer);
					taskCompletableButton.transform.parent = this.taskArea.transform;
					taskCompletableButton.OnButtonClicked += this.ButtonClicked;
					taskCompletableButton.name = ((num >= 10) ? num.ToString() : ("0" + num.ToString()));
					num++;
				}
			}
			this.taskArea.Layout();
		}

		private void ButtonClicked(TaskCompletableButton taskCompletableButton)
		{
			this.CheatToTaskClicked(taskCompletableButton.Task);
		}

		public Action<MapTask> CheatToTaskClicked { get; set; }

		[SerializeField]
		private TaskCompletableButton taskCompletableButtonPrefab;

		[SerializeField]
		private UILayout taskArea;
	}
}
