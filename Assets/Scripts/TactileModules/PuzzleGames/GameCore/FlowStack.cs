using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace TactileModules.PuzzleGames.GameCore
{
	public class FlowStack : IFlowStack
	{
		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event FlowStack.ChangedDelegate Changed;



		public IFlow Top { get; private set; }

		public void Push(IFlow c)
		{
			this.stack.Add(c);
			this.ChangeCurrent(c);
			this.fiberRunner.Run(this.RunAndPop(c), false);
		}

		public IEnumerator PushAndWait(IFlow c)
		{
			this.stack.Add(c);
			this.ChangeCurrent(c);
			bool finished = false;
			this.fiberRunner.Run(this.RunAndCallback(c, delegate
			{
				this.Pop();
				finished = true;
			}), false);
			while (!finished)
			{
				yield return null;
			}
			yield break;
		}

		public T Find<T>() where T : IFlow
		{
			foreach (IFlow flow in this.stack)
			{
				if (flow is T)
				{
					return (T)((object)flow);
				}
			}
			return default(T);
		}

		public IEnumerable<IFlow> TraverseStack()
		{
			return this.stack;
		}

		public bool IsFlowInStack<T>() where T : IFlow
		{
			return this.Find<T>() != null;
		}

		private IEnumerator RunAndCallback(IFlow state, Action callback)
		{
			yield return state;
			callback();
			yield break;
		}

		private IEnumerator RunAndPop(IFlow state)
		{
			yield return state;
			this.Pop();
			yield break;
		}

		private void Pop()
		{
			if (this.Top != null)
			{
				this.stack.Remove(this.Top);
				this.ChangeCurrent((this.stack.Count <= 0) ? null : this.stack[this.stack.Count - 1]);
			}
		}

		private void ChangeCurrent(IFlow value)
		{
			INotifiedFlow notifiedFlow = this.Top as INotifiedFlow;
			if (notifiedFlow != null)
			{
				notifiedFlow.Leave(value);
			}
			IFlow top = this.Top;
			this.Top = value;
			this.Changed(this.Top, top);
			INotifiedFlow notifiedFlow2 = this.Top as INotifiedFlow;
			if (notifiedFlow2 != null)
			{
				notifiedFlow2.Enter(top);
			}
		}

		public void TerminateAll()
		{
			while (this.Top != null)
			{
				this.Pop();
			}
			this.fiberRunner.Terminate();
		}

		private readonly List<IFlow> stack = new List<IFlow>();

		private readonly FiberRunner fiberRunner = new FiberRunner();

		public delegate void ChangedDelegate(IFlow newFlow, IFlow oldFlow);
	}
}
