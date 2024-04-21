using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Fibers
{
	public class Fiber : IFiber
	{
		public Fiber()
		{
		}

		public Fiber(FiberBucket bucket)
		{
			this.bucket = bucket;
		}

		public Fiber(IEnumerator main)
		{
			this.Start(main);
		}

		public Fiber(IEnumerator main, FiberBucket bucket)
		{
			this.bucket = bucket;
			this.Start(main);
		}

		public StackTrace StartStackTrace { get; private set; }

		public void Start(IFiberRunnable fiberRunnable)
		{
			this.Start(Fiber.FiberRunnableToEnumerator(fiberRunnable));
		}

		public void Start(IEnumerator main)
		{
			if (!this.IsTerminated)
			{
				this.Terminate();
			}
			this.cor.Push(new Fiber.State(main));
			if (this.bucket != FiberBucket.Manual)
			{
				this.StartStackTrace = new StackTrace(0, true);
				if (this.bucket != FiberBucket.Pool)
				{
					bool flag = this.Step();
					if (flag)
					{
						FiberCtrl.AddFiber(this, this.bucket);
					}
				}
			}
		}

		public void Push(IEnumerator sub)
		{
			if (this.cor.size > 0 && this.cor.Peek().onLeave != null)
			{
				this.cor.Peek().onLeave();
			}
			this.cor.Push(new Fiber.State(sub));
			if (this.IsTerminated && this.bucket != FiberBucket.Manual && this.bucket != FiberBucket.Pool)
			{
				FiberCtrl.AddFiber(this, this.bucket);
			}
		}

		public void GotoState(IEnumerator sub)
		{
			this.Pop(false);
			this.cor.Push(new Fiber.State(sub));
			if (this.IsTerminated && this.bucket != FiberBucket.Manual && this.bucket != FiberBucket.Pool)
			{
				FiberCtrl.AddFiber(this, this.bucket);
			}
		}

		public int Count
		{
			get
			{
				return this.cor.size;
			}
		}

		public void StepUntilTerminated()
		{
			while (this.Step())
			{
			}
		}

		public bool Step()
		{

            object obj;
			for (;;)
			{
				while (this.cor.size > 0 && !this.cor.Peek().code.MoveNext())
				{
					if (this.cor.size > 0)
					{
						this.Pop(false);
					}
				}
				if (this.cor.size <= 0)
				{
					break;
				}
				obj = this.cor.Peek().code.Current;
				if (obj == null)
				{
					break;
				}
				if (obj is IEnumerator)
				{
					if (this.cor.size > 0 && this.cor.Peek().onLeave != null)
					{
						this.cor.Peek().onLeave();
					}
					this.cor.Push(new Fiber.State((IEnumerator)obj));
				}
				else
				{
                    

					if (obj is IFiberRunnable)
					{
                        if (this.cor.size > 0 && this.cor.Peek().onLeave != null)
                        {
                            this.cor.Peek().onLeave();
                        }
                        IFiberRunnable fiberRunnable = (IFiberRunnable)obj;
                        this.cor.Push(new Fiber.State(fiberRunnable.Run()));
                        this.cor.Peek().onExit = new Fiber.OnExitHandler(fiberRunnable.OnExit);
                        return this.cor.size > 0;
                    }
					if (obj is Fiber.OnExit)
					{
						this.cor.Peek().onExit = ((Fiber.OnExit)obj).handler;
					}
					else if (obj is Fiber.OnLeave)
					{
						this.cor.Peek().onLeave = ((Fiber.OnLeave)obj).handler;
					}
					else if (obj is Fiber.OnEnter)
					{
						this.cor.Peek().onEnter = ((Fiber.OnEnter)obj).handler;
					}
					else if (obj is Fiber.OnTerminate)
					{
						this.cor.Peek().onTerminate = ((Fiber.OnTerminate)obj).handler;
					}
					else if (obj is Fiber.Goto)
					{
						this.Pop(false);
						this.cor.Push(new Fiber.State(((Fiber.Goto)obj).code));
					}
					else if (obj is Fiber.Wait)
					{
						this.cor.Push(new Fiber.State(((Fiber.Wait)obj).Loop()));
					}
					else
					{
						if (obj is WWW)
						{
                            this.cor.Push(new Fiber.State(new Fiber.WWWRunner((WWW)obj).Loop()));
                            return this.cor.size > 0;
                        }
						if (obj is AsyncOperation)
						{
                            Fiber.AsyncOperationRunner asyncOperationRunner = new Fiber.AsyncOperationRunner((AsyncOperation)obj);
                            this.cor.Push(new Fiber.State(asyncOperationRunner.Loop()));
                        }
						if (!(obj is Fiber.DetectFiber))
						{
							break;
						}
						((Fiber.DetectFiber)obj).handler();
					}
				}
			}
			
			return this.cor.size > 0;
		}

		private void Pop(bool terminating = false)
		{
			if (this.cor == null || this.cor.size == 0)
			{
				return;
			}
			Fiber.State state = this.cor.Pop();
			if (state.onExit != null)
			{
				state.onExit();
			}
			if (terminating && state.onTerminate != null)
			{
				state.onTerminate();
			}
			if (this.cor.size > 0 && this.cor.Peek().onEnter != null)
			{
				this.cor.Peek().onEnter();
			}
		}

		public static void TerminateIfAble(Fiber f)
		{
			if (f != null && !f.IsTerminated)
			{
				f.Terminate();
			}
		}

		public static IEnumerator FiberRunnableToEnumerator(IFiberRunnable e)
		{
			yield return e;
			yield break;
		}

		public void Terminate()
		{
			while (this.cor.size > 0)
			{
				this.Pop(true);
			}
		}

		public bool IsTerminated
		{
			get
			{
				return this.cor.size <= 0;
			}
		}

		public void Log()
		{
			if (this.cor.size > 0)
			{
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" - ");
			for (int i = 0; i < this.cor.size; i++)
			{
				Fiber.State state = this.cor[i];
				if (i > 0)
				{
					stringBuilder.Append("   ");
				}
				for (int j = 0; j < i; j++)
				{
					stringBuilder.Append("  ");
				}
				stringBuilder.Append((i + 1).ToString());
				stringBuilder.Append(": ");
				if (state.code == null)
				{
					stringBuilder.Append("empty ");
				}
				else
				{
					stringBuilder.Append(state.code.ToString());
					stringBuilder.Append(" ");
				}
				if (state.onEnter != null)
				{
					stringBuilder.Append("\n onEnter = ");
					stringBuilder.Append(state.onEnter.ToString());
				}
				if (state.onExit != null)
				{
					stringBuilder.Append("\n onExit = ");
					stringBuilder.Append(state.onExit.ToString());
				}
				if (state.onLeave != null)
				{
					stringBuilder.Append("\n onLeave = ");
					stringBuilder.Append(state.onLeave.ToString());
				}
				if (state.onTerminate != null)
				{
					stringBuilder.Append("\n onTerminate = ");
					stringBuilder.Append(state.onTerminate.ToString());
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public string ToShortString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.cor.size; i++)
			{
				Fiber.State state = this.cor[i];
				if (state.code == null)
				{
					stringBuilder.Append("empty ");
				}
				else
				{
					stringBuilder.Append(state.code.ToString());
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		public FiberBucket bucket = FiberBucket.Update;

		private BetterList<Fiber.State> cor = new BetterList<Fiber.State>();

		public delegate void OnExitHandler();

		public class DetectFiber
		{
			public DetectFiber(Fiber.OnExitHandler handler)
			{
				this.handler = handler;
			}

			public Fiber.OnExitHandler handler;
		}

		public class OnTerminate
		{
			public OnTerminate(Fiber.OnExitHandler handler)
			{
				this.handler = handler;
			}

			public Fiber.OnExitHandler handler;
		}

		public class OnExit
		{
			public OnExit(Fiber.OnExitHandler handler)
			{
				this.handler = handler;
			}

			public Fiber.OnExitHandler handler;
		}

		public class OnLeave
		{
			public OnLeave(Fiber.OnExitHandler handler)
			{
				this.handler = handler;
			}

			public Fiber.OnExitHandler handler;
		}

		public class OnEnter
		{
			public OnEnter(Fiber.OnExitHandler handler)
			{
				this.handler = handler;
			}

			public Fiber.OnExitHandler handler;
		}

		public class Goto
		{
			public Goto(IEnumerator code)
			{
				this.code = code;
			}

			public IEnumerator code;
		}

		public class State
		{
			public State(IEnumerator code)
			{
				this.code = code;
				this.onExit = null;
				this.onLeave = null;
				this.onEnter = null;
			}

			public IEnumerator code;

			public Fiber.OnExitHandler onExit;

			public Fiber.OnExitHandler onLeave;

			public Fiber.OnExitHandler onEnter;

			public Fiber.OnExitHandler onTerminate;
		}

		public class Wait
		{
			public Wait(float timeToWait)
			{
				this.TimeToWait = timeToWait;
				this.clickToFinish = false;
			}

			public Wait(float timeToWait, bool clickToFinish)
			{
				this.TimeToWait = timeToWait;
				this.clickToFinish = clickToFinish;
			}

			public IEnumerator Loop()
			{
				while (this.TimeToWait > 0f)
				{
					this.TimeToWait -= Time.deltaTime;
					if (this.clickToFinish && (UnityEngine.Input.touchCount > 0 || Input.GetMouseButtonDown(0) || UnityEngine.Input.GetKeyDown(KeyCode.Space)))
					{
						break;
					}
					yield return null;
				}
				yield break;
			}

			public float TimeToWait;

			private bool clickToFinish;
		}

		private class WWWRunner
		{
			public WWWRunner(WWW www)
			{
				this.www = www;
			}

			public IEnumerator Loop()
			{
				while (!this.www.isDone)
				{
					yield return null;
				}
				yield break;
			}

			private WWW www;
		}

		private class AsyncOperationRunner
		{
			public AsyncOperationRunner(AsyncOperation async)
			{
				this.async = async;
			}

			public IEnumerator Loop()
			{
				while (!this.async.isDone)
				{
					yield return null;
				}
				yield break;
			}

			private AsyncOperation async;
		}
	}
}
