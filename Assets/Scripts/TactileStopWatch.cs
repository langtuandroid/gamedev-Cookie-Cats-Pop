using System;
using System.Diagnostics;
using UnityEngine;

public struct TactileStopWatch
{
	public static TactileStopWatch StartNew(string name)
	{
		return new TactileStopWatch
		{
			name = "StopWatch: " + name,
			watch = Stopwatch.StartNew()
		};
	}

	public void End()
	{
		this.watch.Stop();
		UnityEngine.Debug.Log(this.name + " " + this.watch.Elapsed.TotalMilliseconds.ToString());
	}

	private string name;

	private Stopwatch watch;
}
