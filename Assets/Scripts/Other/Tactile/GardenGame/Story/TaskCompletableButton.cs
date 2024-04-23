using System;
using System.Collections;
using System.Diagnostics;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class TaskCompletableButton : ExtensibleVisual<ITaskCompletableButtonExtension>
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<TaskCompletableButton> OnButtonClicked;



		public MapTask Task { get; private set; }

		public void Initialize(MapTask t, ITimedTask timedTask)
		{
			this.Task = t;
			this.timedTask = timedTask;
			this.numStars.text = t.StarsRequired.ToString();
			this.image.SetTexture(t.Icon);
			this.title.text = L.Get(t.Title);
			this.timerLabel.gameObject.SetActive(timedTask.HasTimer);
			this.timerLabel.text = timedTask.GetFormattedTimeRemaining();
			if (timedTask.HasTimer && timedTask.IsTimerStarted())
			{
				this.priceButton.SetActive(false);
				this.freeButton.SetActive(false);
				this.skipButtonLabel.text = string.Format(L.Get("Finish [C] {0}"), timedTask.CoinSkipCost);
				this.UpdateTimedTaskState();
				this.timerUpdateFiber.Start(this.TimerUpdateLoop());
			}
			else
			{
				this.priceButton.SetActive(t.StarsRequired > 0);
				this.freeButton.SetActive(t.StarsRequired <= 0);
				this.timerCompleteButton.SetActive(false);
				this.skipTimerButton.SetActive(false);
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

		private void UpdateTimedTaskState()
		{
			bool flag = this.timedTask.IsTimerComplete();
			this.skipTimerButton.SetActive(!flag);
			this.timerCompleteButton.SetActive(flag);
			this.timerLabel.text = this.timedTask.GetFormattedTimeRemaining();
		}

		private void CompleteButtonClicked(UIEvent e)
		{
			this.OnButtonClicked(this);
		}

		public Transform StarTargetTransform
		{
			get
			{
				return this.starTargetTransform;
			}
		}

		public IEnumerator Animate(Vector3 endPosition, float delay)
		{
			if (base.Extension != null)
			{
				yield return base.Extension.Animate(endPosition, delay);
			}
			yield break;
		}

		[SerializeField]
		private UILabel numStars;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UITextureQuad image;

		[SerializeField]
		private Transform starTargetTransform;

		[SerializeField]
		private GameObject priceButton;

		[SerializeField]
		private GameObject freeButton;

		[SerializeField]
		private GameObject timerCompleteButton;

		[SerializeField]
		private GameObject skipTimerButton;

		[SerializeField]
		private UILabel timerLabel;

		[SerializeField]
		private UILabel skipButtonLabel;

		private Fiber timerUpdateFiber = new Fiber();

		private ITimedTask timedTask;
	}
}
