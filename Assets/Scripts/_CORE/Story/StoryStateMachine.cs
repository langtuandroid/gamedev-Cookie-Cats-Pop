using System;
using System.Collections;
using Fibers;
using UnityEngine;

public abstract class StoryStateMachine : MonoBehaviour
{
	private void Start()
	{
		this.StartMachine();
	}

	private void OnDestroy()
	{
		this.StopMachine();
	}

	public void StartMachine()
	{
		if (this.fiber == null)
		{
			this.fiber = new Fiber(FiberBucket.Manual);
		}
		this.fiber.Start(this.Flow());
	}

	public void StopMachine()
	{
		if (this.fiber != null)
		{
			this.fiber.Terminate();
		}
	}

	private void Update()
	{
		if (this.fiber != null)
		{
			this.fiber.Step();
		}
	}

	private IEnumerator Flow()
	{
		StoryState[] states = this.GetStates();
		foreach (StoryState storyState in states)
		{
			storyState.Initialize();
		}
		yield return new Fiber.OnExit(delegate()
		{
			foreach (StoryState storyState2 in states)
			{
				storyState2.Destroy();
			}
		});
		foreach (StoryState state in states)
		{
			state.Enter();
			yield return state.Logic();
			state.Exit();
		}
		yield break;
	}

	protected abstract StoryState[] GetStates();

	private Fiber fiber;
}
