using System;
using System.Collections;
using Fibers;
using TactileModules.Foundation;
using UnityEngine;

namespace TactileModules.AgeInfo.UI
{
	public class AgeInfoIncrementButton : MonoBehaviour
	{
		private int CurrentValue
		{
			get
			{
				return (!this.IsNegative) ? this.currentValue : (this.currentValue * -1);
			}
			set
			{
				this.currentValue = value;
			}
		}

		private void Start()
		{
			ApplicationLifeCycle.ApplicationWillEnterForeground += this.ApplicationWillEnterForeground;
		}

		private void ApplicationWillEnterForeground()
		{
			this.pressed = false;
			this.pressedTime = 0f;
			this.currentValue = 1;
		}

		private void OnPress(bool pressed)
		{
			this.pressed = pressed;
			if (pressed)
			{
				if (this.OnValueIncrement != null)
				{
					this.OnValueIncrement(this.CurrentValue);
				}
				this.loopFiber.Start(this.IncrementLoop());
			}
			else
			{
				this.loopFiber.Terminate();
				this.pressedTime = 0f;
				this.CurrentValue = 1;
			}
		}

		private void OnDestroy()
		{
			this.loopFiber.Terminate();
		}

		private IEnumerator IncrementLoop()
		{
			yield return FiberHelper.Wait(0.5f, (FiberHelper.WaitFlag)0);
			while (this.pressed)
			{
				yield return FiberHelper.Wait(0.1f, (FiberHelper.WaitFlag)0);
				if (this.OnValueIncrement != null)
				{
					this.OnValueIncrement(this.CurrentValue);
				}
				this.pressedTime += 0.1f;
				if (this.pressedTime >= 2f)
				{
					this.pressedTime = 0f;
					if (this.currentValue == 1)
					{
						this.CurrentValue = 2;
					}
					else
					{
						this.currentValue += 2;
					}
				}
			}
			yield break;
		}

		[SerializeField]
		private bool IsNegative;

		public Action<int> OnValueIncrement;

		private bool pressed;

		private Fiber loopFiber = new Fiber();

		private float pressedTime;

		private const float TimeInitialWait = 0.5f;

		private const float TimeIncrement = 0.1f;

		private const float StepIncrement = 2f;

		private const int ValueIncrement = 2;

		private int currentValue = 1;
	}
}
