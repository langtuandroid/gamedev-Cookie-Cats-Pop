using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Path-o-logical/PoolManager/PreloadOverTime")]
public class PreloadOverTime : MonoBehaviour
{
	private void Start()
	{
		base.StartCoroutine(this.Preload());
	}

	private IEnumerator Preload()
	{
		yield return new WaitForSeconds(this.delaySeconds);
		SpawnPool pool = PoolManager.Pools[this.poolName];
		int amount = this.preloadTotal - pool.GetPrefabPool(this.prefab.name).totalCount;
		if (amount <= 0)
		{
			yield break;
		}
		int numPerFrame = amount / this.durrationFrames;
		int remainder = amount % this.durrationFrames;
		bool isFirst = true;
		for (int i = 0; i < this.durrationFrames; i++)
		{
			int numThisFrame = numPerFrame;
			if (isFirst)
			{
				numThisFrame += remainder;
				isFirst = false;
			}
			for (int j = 0; j < numThisFrame; j++)
			{
				Transform inst = pool.GetPrefabPool(this.prefab.name).SpawnNew();
				if (inst != null)
				{
					pool.Despawn(inst);
				}
				yield return null;
			}
			if (pool.GetPrefabPool(this.prefab.name).totalCount > this.preloadTotal)
			{
				yield break;
			}
		}
		yield break;
	}

	public string poolName;

	public Transform prefab;

	public int preloadTotal = 2;

	public int durrationFrames = 2;

	public float delaySeconds;
}
