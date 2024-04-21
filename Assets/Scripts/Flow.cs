using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

public abstract class Flow
{
	public static Flow Current { get; private set; }

	public static void Start(Flow newFlow)
	{
		if (Flow.queue.Count > 0)
		{
			return;
		}
		Flow.TerminateQueue();
		Flow.Enqueue(newFlow);
	}

	public static void Enqueue(Flow newFlow)
	{
		Flow.queue.Add(newFlow);
		Flow.EnsureLoop();
	}

	public static void TerminateQueue()
	{
		Flow.queueFiber.Terminate();
	}

	private static IEnumerator ProcessQueue()
	{
		for (;;)
		{
			if (Flow.queue.Count > 0)
			{
				Flow flow = Flow.queue[0];
				Flow.Current = flow;
				yield return flow.Logic();
				Flow.Current = null;
				Flow.queue.RemoveAt(0);
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	private static void EnsureLoop()
	{
		if (Flow.queueFiber.IsTerminated)
		{
			Flow.queueFiber.Start(Flow.ProcessQueue());
		}
	}

	public static bool Step()
	{
		return Flow.queueFiber.Step();
	}

	public static void TerminateAndClearQueue()
	{
		Flow.Current = null;
		Flow.queueFiber.Terminate();
		Flow.queue.Clear();
	}

	protected abstract IEnumerator Logic();

	private static List<Flow> queue = new List<Flow>();

	private static Fiber queueFiber = new Fiber();
}
