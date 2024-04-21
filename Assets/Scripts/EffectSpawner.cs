using System;
using System.Collections;
using System.Collections.Generic;
using Fibers;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
	private void Start()
	{
		foreach (EffectSpawner.Spawner spawner in this.spawners)
		{
			spawner.Initialize();
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.spawners.Length; i++)
		{
			this.spawners[i].Step();
		}
	}

	private void OnDisable()
	{
		foreach (EffectSpawner.Spawner spawner in this.spawners)
		{
			spawner.Destroy();
		}
	}

	public EffectSpawner.Spawner[] spawners;

	[Serializable]
	public class Spawner
	{
		public void Initialize()
		{
			this.spawnFiber = new Fiber(this.SpawnLogic(), FiberBucket.Manual);
			this.spawnedEffects = new List<SpawnedEffect>();
		}

		public void Destroy()
		{
			if (this.spawnedEffects == null)
			{
				return;
			}
			foreach (SpawnedEffect spawnedEffect in this.spawnedEffects)
			{
				spawnedEffect.Terminate();
			}
			this.spawnedEffects.Clear();
		}

		public void Step()
		{
			if (this.spawnFiber == null)
			{
				return;
			}
			this.spawnFiber.Step();
		}

		private IEnumerator SpawnLogic()
		{
			for (;;)
			{
				yield return FiberHelper.Wait(UnityEngine.Random.Range(this.minFrequency, this.maxFrequency), (FiberHelper.WaitFlag)0);
				SpawnedEffect effect = EffectPool.Instance.SpawnEffect(this.Effect, Vector3.zero, this.spawnLocation.gameObject.layer, new object[0]);
				effect.transform.position = this.spawnLocation.position;
				UIElement locationElement = this.spawnLocation.gameObject.GetElement();
				if (locationElement != null)
				{
					UIElement element = effect.gameObject.GetElement();
					if (element != null)
					{
						element.SetSizeAndDoLayout(locationElement.Size);
					}
				}
				this.spawnedEffects.Add(effect);
				effect.onFinished = delegate()
				{
					this.spawnedEffects.Remove(effect);
				};
			}
			yield break;
		}

		public string Effect;

		public Transform spawnLocation;

		public float minFrequency;

		public float maxFrequency;

		private Fiber spawnFiber;

		private List<SpawnedEffect> spawnedEffects;
	}
}
