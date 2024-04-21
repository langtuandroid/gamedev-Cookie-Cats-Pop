using System;
using System.Collections;
using Fibers;
using UnityEngine;

public class DucklingGrooveScaler : MonoBehaviour
{
	private void Start()
	{
		this.fiber = new Fiber(FiberBucket.Manual);
		this.fiber.Start(this.Flow());
	}

	private void Update()
	{
		this.fiber.Step();
	}

	private IEnumerator Flow()
	{
		yield return FiberHelper.Wait(UnityEngine.Random.value * (60f / this.bpm), (FiberHelper.WaitFlag)0);
		yield break;
	}

	public float bpm = 100f;

	private Fiber fiber;
}
