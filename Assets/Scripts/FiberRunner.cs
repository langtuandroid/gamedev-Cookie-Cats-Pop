using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;

public class FiberRunner
{
	public FiberRunner() : this(FiberBucket.Update)
	{
	}

	public FiberRunner(FiberBucket bucket)
	{
		this.bucket = bucket;
		this.available = new List<Fiber>();
		this.fibers = new List<Fiber>();
	}

	public void Run(IEnumerator code, bool stepOnce = false)
	{
		this.RemoveAllTerminatedFibers();
		Fiber newFiber = this.GetNewFiber();
		this.fibers.Add(newFiber);
		newFiber.Start(code);
		if (stepOnce)
		{
			newFiber.Step();
		}
	}

	public void Terminate()
	{
		for (int i = 0; i < this.fibers.Count; i++)
		{
			this.fibers[i].Terminate();
		}
		this.RemoveAllTerminatedFibers();
		this.available.Clear();
	}

	public bool IsDone()
	{
		for (int i = 0; i < this.fibers.Count; i++)
		{
			if (!this.fibers[i].IsTerminated)
			{
				return false;
			}
		}
		return true;
	}

	public void Step()
	{
		for (int i = 0; i < this.fibers.Count; i++)
		{
			this.fibers[i].Step();
		}
	}

	public Fiber Pop()
	{
		int count = this.available.Count;
		Fiber result = this.available[count - 1];
		this.available.RemoveAt(count - 1);
		return result;
	}

	private Fiber GetNewFiber()
	{
		return (this.available.Count <= 0) ? new Fiber(this.bucket) : this.Pop();
	}

	private void RemoveAllTerminatedFibers()
	{
		for (int i = 0; i < this.fibers.Count; i++)
		{
			if (this.fibers[i].IsTerminated)
			{
				this.available.Add(this.fibers[i]);
				this.fibers[i] = null;
				this.fibers.RemoveAt(i);
				i--;
			}
		}
	}

	private List<Fiber> available;

	private List<Fiber> fibers;

	private FiberBucket bucket;
}
