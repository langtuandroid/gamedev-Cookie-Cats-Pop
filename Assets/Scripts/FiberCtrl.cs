using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fibers;
using UnityEngine;

public class FiberCtrl : MonoBehaviour
{
	private static FiberCtrl instance
	{
		get
		{
			if (FiberCtrl._instance == null)
			{
				FiberCtrl._instance = new GameObject().AddComponent<FiberCtrl>();
				FiberCtrl._instance.name = "FiberCtrl";
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(FiberCtrl._instance.gameObject);
				}
			}
			return FiberCtrl._instance;
		}
	}

	public static void AddFiber(Fiber fiber, FiberBucket bucket)
	{
		if (!FiberCtrl.instance.fibers.ContainsKey(bucket))
		{
			FiberCtrl.instance.fibers[bucket] = new List<Fiber>();
		}
		if (FiberCtrl.instance.fibers[bucket].Contains(fiber))
		{
			return;
		}
		FiberCtrl.instance.fibers[bucket].Add(fiber);
	}

	public static void WriteLogStatic()
	{
		FiberCtrl.instance.WriteLog();
	}

	public void WriteLog()
	{
		this.WriteLogBucket(FiberBucket.Update);
	}

	public void WriteLogBucket(FiberBucket bucket)
	{
		if (!this.fibers.ContainsKey(bucket))
		{
			return;
		}
		List<Fiber> list = this.fibers[bucket];
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Log();
		}
	}

	public static FiberCtrl.FiberPool Pool
	{
		get
		{
			return FiberCtrl.instance.pool;
		}
	}

	private void StepBucket(FiberBucket bucket)
	{
		List<Fiber> list;
		if (this.fibers.TryGetValue(bucket, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					if (!list[i].Step())
					{
						list.RemoveAt(i);
						i--;
					}
				}
				catch (Exception innerException)
				{
					string additionalInfo = list[i].ToString();
					throw new FiberCtrl.FiberUnhandledException(list[i].StartStackTrace, innerException, additionalInfo);
				}
			}
		}
	}

	private void Update()
	{
		this.StepBucket(FiberBucket.Update);
		this.pool.Step();
	}

	private void Terminate()
	{
		foreach (KeyValuePair<FiberBucket, List<Fiber>> keyValuePair in this.fibers)
		{
			foreach (Fiber fiber in keyValuePair.Value)
			{
				fiber.Terminate();
			}
			keyValuePair.Value.Clear();
		}
	}

	public static void ForceUpdate()
	{
		FiberCtrl.instance.Update();
	}

	public static void TerminateAll()
	{
		FiberCtrl.instance.Terminate();
	}

	private void WriteBucketString(StringBuilder builder, FiberBucket bucket, List<Fiber> fibers)
	{
		builder.Append(bucket.ToString());
		builder.Append(" Bucket: ");
		builder.AppendLine();
		for (int i = 0; i < fibers.Count; i++)
		{
			builder.Append(fibers[i].ToString());
			builder.AppendLine();
		}
	}

	public string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("FiberCtrl:");
		stringBuilder.AppendLine();
		stringBuilder.Append(this.pool.ToString());
		foreach (KeyValuePair<FiberBucket, List<Fiber>> keyValuePair in this.fibers)
		{
			this.WriteBucketString(stringBuilder, keyValuePair.Key, keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	public string ToShortString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<FiberBucket, List<Fiber>> keyValuePair in this.fibers)
		{
			stringBuilder.Append(keyValuePair.Key.ToString() + ":");
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				stringBuilder.Append(keyValuePair.Value[i].ToShortString());
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToStringStatic()
	{
		return FiberCtrl.instance.ToString();
	}

	public static string ToShortStringStatic()
	{
		return FiberCtrl.instance.ToShortString();
	}

	private static FiberCtrl _instance;

	private FiberCtrl.FiberPool pool = new FiberCtrl.FiberPool();

	private Dictionary<FiberBucket, List<Fiber>> fibers = new Dictionary<FiberBucket, List<Fiber>>(FiberCtrl.FiberBucketComparer.Instance);

	public class FiberUnhandledException : Exception
	{
		public FiberUnhandledException(StackTrace startFiberStack, Exception innerException, string additionalInfo = "") : base("\n" + startFiberStack + ((!string.IsNullOrEmpty(additionalInfo)) ? ("\n ---FIBER INFO--- \n " + additionalInfo) : string.Empty), innerException)
		{
		}
	}

	public class FiberPool
	{
		public Fiber AllocateFiber()
		{
			return this.AllocateFiber(FiberBucket.Pool);
		}

		public Fiber AllocateFiber(FiberBucket bucket)
		{
			Fiber fiber = (this.available.size <= 0) ? new Fiber() : this.available.Pop();
			fiber.bucket = bucket;
			return fiber;
		}

		public Fiber AllocateFiber(IEnumerator e)
		{
			return this.AllocateFiber(e, FiberBucket.Update);
		}

		public Fiber AllocateFiber(IEnumerator e, FiberBucket bucket)
		{
			Fiber fiber = this.AllocateFiber(bucket);
			fiber.Start(e);
			return fiber;
		}

		public void ReleaseFiber(ref Fiber fiber)
		{
			if (fiber != null)
			{
				Fiber.TerminateIfAble(fiber);
				this.available.Push(fiber);
				fiber = null;
			}
		}

		public void Run(IFiberRunnable e, bool stepOneTime = false)
		{
			this.Run(Fiber.FiberRunnableToEnumerator(e), stepOneTime);
		}

		public void Run(IEnumerator e, bool stepOneTime = false)
		{
			Fiber fiber = this.AllocateFiber(e, FiberBucket.Pool);
			this.running.Push(fiber);
			if (stepOneTime && !fiber.Step())
			{
				this.available.Push(fiber);
				this.running.Pop();
			}
		}

		public void Step()
		{
			for (int i = 0; i < this.running.size; i++)
			{
				try
				{
					if (!this.running[i].Step())
					{
						this.available.Push(this.running[i]);
						this.running.RemoveAt(i);
						i--;
					}
				}
				catch (Exception innerException)
				{
					string additionalInfo = this.running[i].ToString();
					throw new FiberCtrl.FiberUnhandledException(this.running[i].StartStackTrace, innerException, additionalInfo);
				}
			}
		}

		public string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("FiberPool: \n");
			for (int i = 0; i < this.running.size; i++)
			{
				stringBuilder.Append(this.running[i].ToString());
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public string ToShortString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.running.size; i++)
			{
				stringBuilder.Append(this.running[i].ToShortString() + ", ");
			}
			return stringBuilder.ToString();
		}

		private BetterList<Fiber> running = new BetterList<Fiber>();

		private BetterList<Fiber> available = new BetterList<Fiber>();
	}

	private class FiberBucketComparer : IEqualityComparer<FiberBucket>
	{
		private FiberBucketComparer()
		{
		}

		public bool Equals(FiberBucket x, FiberBucket y)
		{
			return x == y;
		}

		public int GetHashCode(FiberBucket obj)
		{
			return (int)obj;
		}

		public static readonly FiberCtrl.FiberBucketComparer Instance = new FiberCtrl.FiberBucketComparer();
	}
}
