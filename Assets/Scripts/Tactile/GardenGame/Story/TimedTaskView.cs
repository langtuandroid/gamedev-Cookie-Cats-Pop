using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class TimedTaskView : ExtensibleView<TimedTaskView.IExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action SkipClicked;



		public Transform CoinTarget
		{
			get
			{
				return this.skipTimerButton.transform;
			}
		}

		public void Initialize(MapTask task, ITimedTask timedTask)
		{
			this.timedTask = timedTask;
			this.UpdateTimedTaskState();
			this.timerUpdateFiber.Start(this.TimerUpdateLoop());
			if (base.Extension != null)
			{
				base.Extension.Initialize(task, timedTask);
			}
		}

		private void OnDestroy()
		{
			this.timerUpdateFiber.Terminate();
		}

		private IEnumerator TimerUpdateLoop()
		{
			for (;;)
			{
				this.UpdateTimedTaskState();
				yield return null;
			}
			yield break;
		}

		private void SetButtonLabel(string text)
		{
			UILabel componentInChildren = this.skipTimerButton.GetComponentInChildren<UILabel>();
			if (componentInChildren != null)
			{
				componentInChildren.text = text;
			}
		}

		private void UpdateTimedTaskState()
		{
			bool flag = this.timedTask.IsTimerComplete();
			string buttonLabel = (!flag) ? string.Format(L.Get("Finish [C] {0}"), this.timedTask.CoinSkipCost) : "Finish";
			this.SetButtonLabel(buttonLabel);
			this.timerLabel.text = this.timedTask.GetFormattedTimeRemaining();
		}

		private void CloseClick(UIEvent e)
		{
			base.Close(0);
		}

		private void SkipClick(UIEvent e)
		{
			this.SkipClicked();
		}

		[SerializeField]
		private GameObject skipTimerButton;

		[SerializeField]
		private UILabel timerLabel;

		private ITimedTask timedTask;

		private Fiber timerUpdateFiber = new Fiber();

		public interface IExtension
		{
			void Initialize(MapTask task, ITimedTask timedTask);
		}
	}
}
