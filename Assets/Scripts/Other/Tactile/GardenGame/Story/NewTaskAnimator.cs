using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

namespace Tactile.GardenGame.Story
{
	public class NewTaskAnimator : ExtensibleVisual<NewTaskAnimator.IExtension>
	{
		public void StartAnimatingNewTasks(List<MapTask> tasks, float waitTime, bool dontFadeAway)
		{
			List<MapTask> tasks2 = new List<MapTask>(tasks);
			this.fiber.Start(this.AnimateNewTasks(tasks2, waitTime, dontFadeAway));
		}

		private IEnumerator AnimateNewTasks(List<MapTask> tasks, float waitTime, bool dontFadeAway)
		{
			yield return FiberHelper.Wait(waitTime, (FiberHelper.WaitFlag)0);
			this.Enabled = true;
			yield return new Fiber.OnExit(delegate()
			{
				this.Enabled = false;
			});
			foreach (MapTask task in tasks)
			{
				yield return this.AnimateUnlockedTask(task, dontFadeAway);
			}
			yield break;
		}

		private IEnumerator AnimateUnlockedTask(MapTask task, bool dontFadeAway)
		{
			this.bubble.Initialize(task);
			yield return base.Extension.AnimateNewTask(task, dontFadeAway);
			yield break;
		}

		private void Update()
		{
			if (this.fiber != null)
			{
				this.fiber.Step();
			}
		}

		public void StopAnimating()
		{
			if (this.fiber != null)
			{
				this.fiber.Terminate();
			}
		}

		public bool Enabled
		{
			get
			{
				return this.pivot.activeSelf;
			}
			set
			{
				this.pivot.SetActive(value);
				this.button.SetActive(value);
			}
		}

		public void DisableAnimationPivot()
		{
			this.pivot.SetActive(false);
		}

		[SerializeField]
		private NewTaskBubble bubble;

		[SerializeField]
		private GameObject pivot;

		[SerializeField]
		private GameObject button;

		private readonly Fiber fiber = new Fiber(FiberBucket.Manual);

		public interface IExtension
		{
			IEnumerator AnimateNewTask(MapTask newTask, bool dontFadeAway);
		}
	}
}
