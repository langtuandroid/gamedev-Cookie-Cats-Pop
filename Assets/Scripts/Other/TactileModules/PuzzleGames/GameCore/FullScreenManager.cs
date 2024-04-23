using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

namespace TactileModules.PuzzleGames.GameCore
{
	public class FullScreenManager : IFullScreenManager
	{
		public FullScreenManager(IFullScreenTransition transition)
		{
			this.transition = transition;
			this.PreChange = new HookList<ChangeInfo>();
			this.PostChange = new HookList<ChangeInfo>();
			this.MidChange = new HookList<ChangeInfo>();
		}

		public IHookList<ChangeInfo> PreChange { get; private set; }

		public IHookList<ChangeInfo> PostChange { get; private set; }

		public IHookList<ChangeInfo> MidChange { get; private set; }

		public IFullScreenOwner Top { get; private set; }

		public IEnumerator Push(IFullScreenOwner c)
		{
			this.stack.Add(c);
			yield return this.ChangeWithTransition(c);
			yield break;
		}

		public IEnumerator Pop()
		{
			if (this.Top != null)
			{
				this.stack.Remove(this.Top);
				yield return this.ChangeWithTransition((this.stack.Count <= 0) ? null : this.stack[this.stack.Count - 1]);
			}
			yield break;
		}

		public void PushInstantly(IFullScreenOwner c)
		{
			this.stack.Add(c);
			this.ChangeCurrent(c);
		}

		public void PopInstantly()
		{
			if (this.Top != null)
			{
				this.stack.Remove(this.Top);
				this.ChangeCurrent((this.stack.Count <= 0) ? null : this.stack[this.stack.Count - 1]);
			}
		}

		public void PrepareStackWithoutNotifications(params IFullScreenOwner[] screenOwners)
		{
			this.stack.Clear();
			this.stack.AddRange(screenOwners);
			this.Top = this.stack.Last<IFullScreenOwner>();
		}

		public void PrepareStackImmediately(params IFullScreenOwner[] screenOwners)
		{
			this.stack.Clear();
			this.stack.AddRange(screenOwners);
			this.Top = this.stack.Last<IFullScreenOwner>();
			this.instantFiber.Start(this.Top.ScreenAcquired());
			this.instantFiber.StepUntilTerminated();
			this.Top.ScreenReady();
		}

		public IEnumerator ChangeToSameScreen()
		{
			yield return this.ChangeWithTransition(this.Top);
			yield break;
		}

		private void ChangeCurrent(IFullScreenOwner value)
		{
			if (this.Top != null)
			{
				this.Top.ScreenLost();
			}
			this.transition.FullyOut();
			this.Top = value;
			if (this.Top != null)
			{
				this.instantFiber.Start(this.Top.ScreenAcquired());
				this.instantFiber.StepUntilTerminated();
				this.Top.ScreenReady();
			}
		}

		private IEnumerator ChangeWithTransition(IFullScreenOwner value)
		{
			ChangeInfo info = new ChangeInfo(this.Top, value);
			yield return this.PreChange.InvokeAll(info);
			yield return this.transition.TransitionOut();
			if (this.Top != null)
			{
				this.Top.ScreenLost();
				if (value == this.Top)
				{
					yield return null;
				}
			}
			this.transition.FullyOut();
			this.Top = value;
			yield return this.MidChange.InvokeAll(info);
			if (this.Top != null)
			{
				yield return this.Top.ScreenAcquired();
			}
			yield return this.transition.TransitionIn();
			yield return this.PostChange.InvokeAll(info);
			if (this.Top != null)
			{
				this.Top.ScreenReady();
			}
			yield break;
		}

		private readonly IFullScreenTransition transition;

		private readonly List<IFullScreenOwner> stack = new List<IFullScreenOwner>();

		private readonly Fiber instantFiber = new Fiber(FiberBucket.Manual);
	}
}
