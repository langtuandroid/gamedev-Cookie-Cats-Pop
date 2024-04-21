using System;
using System.Collections;
using Fibers;
using UnityEngine;

public abstract class SpawnedEffect : MonoBehaviour
{
	protected virtual void OnDestroy()
	{
		EffectPool.Instance.ValidateEffectForDestruction(this);
		this.isDestroyed = true;
	}

	protected void OnDespawned()
	{
		this.onFinished = null;
		FiberCtrl.Pool.ReleaseFiber(ref this.logic);
	}

	public void StartEffect(object[] parameters)
	{
		this.logic = FiberCtrl.Pool.AllocateFiber(this.AnimateAndDespawn(parameters));
	}

	public IEnumerator WaitUntilFinished()
	{
		while (this.logic != null && !this.logic.IsTerminated)
		{
			yield return null;
		}
		yield break;
	}

	protected IEnumerator AnimateAndDespawn(object[] parameters)
	{
		yield return new Fiber.OnExit(delegate()
		{
			EffectPool.Instance.DespawnEffect(base.transform);
		});
		this.Started(parameters);
		yield return this.AnimationLogic(parameters);
		this.Ended();
		if (this.onFinished != null)
		{
			this.onFinished();
		}
		yield break;
	}

	public void Terminate()
	{
		this.onFinished = null;
		if (this.logic != null && !this.isDestroyed)
		{
			this.logic.Terminate();
		}
	}

	protected abstract IEnumerator AnimationLogic(object[] parameters);

	protected virtual void Started(object[] parameters)
	{
	}

	protected virtual void Ended()
	{
	}

	public virtual bool SpawnCountingEnabled
	{
		get
		{
			return true;
		}
	}

	public int PrewarmAmount;

	private Fiber logic;

	public Action onFinished;

	private bool isDestroyed;
}
