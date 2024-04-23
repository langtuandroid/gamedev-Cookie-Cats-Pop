using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class StoryState
{
	public virtual void Initialize()
	{
		this.pivot.SetActive(false);
	}

	public virtual void Destroy()
	{
		this.pivot.SetActive(false);
	}

	public virtual void Enter()
	{
		this.pivot.SetActive(true);
	}

	public virtual void Exit()
	{
		this.pivot.SetActive(false);
	}

	public abstract IEnumerator Logic();

	public GameObject pivot;
}
