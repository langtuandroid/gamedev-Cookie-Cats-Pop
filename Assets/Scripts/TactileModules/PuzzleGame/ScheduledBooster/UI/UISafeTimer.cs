using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.PuzzleGame.ScheduledBooster.UI
{
	public class UISafeTimer
	{
		public UISafeTimer(GameObject owner, Action method, float interval = 1f)
		{
			this.owner = owner;
			this.method = method;
			this.interval = interval;
		}

		public void Run()
		{
			FiberCtrl.Pool.Run(this.Logic(), false);
		}

		private IEnumerator Logic()
		{
			while (this.owner != null)
			{
				this.method();
				yield return FiberHelper.Wait(this.interval, (FiberHelper.WaitFlag)0);
			}
			yield break;
		}

		private readonly GameObject owner;

		private readonly Action method;

		private readonly float interval;
	}
}
