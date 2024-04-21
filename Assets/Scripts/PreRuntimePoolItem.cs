using System;
using UnityEngine;

[AddComponentMenu("Path-o-logical/PoolManager/Pre-Runtime Pool Item")]
public class PreRuntimePoolItem : MonoBehaviour
{
	private void Start()
	{
		SpawnPool spawnPool;
		if (!PoolManager.Pools.TryGetValue(this.poolName, out spawnPool))
		{
			return;
		}
		spawnPool.Add(base.transform, this.prefabName, this.despawnOnStart, !this.doNotReparent);
	}

	public string poolName = string.Empty;

	public string prefabName = string.Empty;

	public bool despawnOnStart = true;

	public bool doNotReparent;
}
