using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
public sealed class SpawnPool : MonoBehaviour, IList<Transform>, IEnumerable, ICollection<Transform>, IEnumerable<Transform>
{
	public Transform group { get; private set; }

	public PrefabPool GetPrefabPool(string name)
	{
		Transform transform;
		this.prefabs.TryGetValue(name, out transform);
		if (transform == null)
		{
			return null;
		}
		return this.GetPrefabPool(transform);
	}

	public PrefabPool GetPrefabPool(Transform prefab)
	{
		for (int i = 0; i < this._prefabPools.size; i++)
		{
			if (this._prefabPools[i].prefab == prefab)
			{
				return this._prefabPools[i];
			}
		}
		return null;
	}

	private void Awake()
	{
		if (this.dontDestroyOnLoad)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		this.group = base.transform;
		if (this.poolName == string.Empty)
		{
			this.poolName = this.group.name.Replace("Pool", string.Empty);
			this.poolName = this.poolName.Replace("(Clone)", string.Empty);
		}
		if (this.logMessages)
		{
		}
		foreach (PrefabPool prefabPool in this._perPrefabPoolOptions)
		{
			if (!(prefabPool.prefab == null))
			{
				prefabPool.inspectorInstanceConstructor();
				this.CreatePrefabPool(prefabPool);
			}
		}
		PoolManager.Pools.Add(this);
	}

	private void OnDestroy()
	{
		if (this.logMessages)
		{
		}
		PoolManager.Pools.Remove(this);
		base.StopAllCoroutines();
		this._spawned.Clear();
		foreach (PrefabPool prefabPool in this._prefabPools)
		{
			prefabPool.SelfDestruct();
		}
		this._prefabPools.Clear();
		this.prefabs._Clear();
	}

	public void CreatePrefabPool(PrefabPool prefabPool)
	{
		if (this.GetPrefab(prefabPool.prefab) == null)
		{
			prefabPool.spawnPool = this;
			this._prefabPools.Add(prefabPool);
			this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
		}
		if (!prefabPool.preloaded)
		{
			if (this.logMessages)
			{
			}
			prefabPool.PreloadInstances();
		}
	}

	public void Add(Transform instance, string prefabName, bool despawn, bool parent)
	{
		foreach (PrefabPool prefabPool in this._prefabPools)
		{
			if (prefabPool.prefabGO == null)
			{
				break;
			}
			if (prefabPool.prefabGO.name == prefabName)
			{
				prefabPool.AddUnpooled(instance, despawn);
				if (this.logMessages)
				{
				}
				if (parent)
				{
					instance.parent = this.group;
				}
				if (!despawn)
				{
					this._spawned.Add(instance);
				}
				break;
			}
		}
	}

	public void Add(Transform item)
	{
		string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
		throw new NotImplementedException(message);
	}

	public void Remove(Transform item)
	{
		string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
		throw new NotImplementedException(message);
	}

	public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
	{
		int i = 0;
		Transform transform;
		while (i < this._prefabPools.size)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.prefabGO == prefab.gameObject)
			{
				transform = prefabPool.SpawnInstance(pos, rot);
				if (transform == null)
				{
					return null;
				}
				if (transform.parent != this.group)
				{
					transform.parent = this.group;
				}
				this._spawned.Add(transform);
				return transform;
			}
			else
			{
				i++;
			}
		}
		PrefabPool prefabPool2 = new PrefabPool(prefab);
		this.CreatePrefabPool(prefabPool2);
		transform = prefabPool2.SpawnInstance(pos, rot);
		transform.parent = this.group;
		this._spawned.Add(transform);
		return transform;
	}

	public Transform Spawn(Transform prefab)
	{
		return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
	}

	public Transform SpawnForceActiveRecursively(Transform prefab)
	{
		return this.SpawnForceActiveRecursively(prefab, Vector3.zero, Quaternion.identity);
	}

	public Transform SpawnForceActiveRecursively(Transform prefab, Vector3 pos, Quaternion rot)
	{
		Transform transform = this.Spawn(prefab, pos, rot);
		if (transform != null)
		{
			transform.gameObject.SetActive(true);
		}
		return transform;
	}

	public void Despawn(Transform xform)
	{
		bool flag = false;
		for (int i = 0; i < this._prefabPools.size; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool._spawned.Contains(xform))
			{
				flag = prefabPool.DespawnInstance(xform);
				break;
			}
			if (prefabPool._despawned.Contains(xform))
			{
				return;
			}
		}
		if (!flag)
		{
			return;
		}
		this._spawned.Remove(xform);
	}

	public void Despawn(Transform instance, float seconds)
	{
		base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds));
	}

	private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		this.Despawn(instance);
		yield break;
	}

	public void DespawnAll()
	{
		for (int i = this._spawned.size - 1; i >= 0; i--)
		{
			this.Despawn(this._spawned[i]);
		}
	}

	public bool IsSpawned(Transform instance)
	{
		return this._spawned.Contains(instance);
	}

	public Transform GetPrefab(Transform prefab)
	{
		for (int i = 0; i < this._prefabPools.size; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.prefabGO == null)
			{
			}
			if (prefabPool.prefabGO == prefab.gameObject)
			{
				return prefabPool.prefab;
			}
		}
		return null;
	}

	public GameObject GetPrefab(GameObject prefab)
	{
		for (int i = 0; i < this._prefabPools.size; i++)
		{
			PrefabPool prefabPool = this._prefabPools[i];
			if (prefabPool.prefabGO == null)
			{
			}
			if (prefabPool.prefabGO == prefab)
			{
				return prefabPool.prefabGO;
			}
		}
		return null;
	}

	public override string ToString()
	{
		List<string> list = new List<string>();
		foreach (Transform transform in this._spawned)
		{
			list.Add(transform.name);
		}
		return string.Join(", ", list.ToArray());
	}

	public Transform this[int index]
	{
		get
		{
			return this._spawned[index];
		}
		set
		{
			throw new NotImplementedException("Read-only.");
		}
	}

	public bool Contains(Transform item)
	{
		string message = "Use IsSpawned(Transform instance) instead.";
		throw new NotImplementedException(message);
	}

	public void CopyTo(Transform[] array, int arrayIndex)
	{
		for (int i = 0; i < this._spawned.size; i++)
		{
			array[arrayIndex + i] = this._spawned[i];
		}
	}

	public int Count
	{
		get
		{
			return this._spawned.size;
		}
	}

	public IEnumerator<Transform> GetEnumerator()
	{
		foreach (Transform instance in this._spawned)
		{
			yield return instance;
		}
		yield break;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		foreach (Transform instance in this._spawned)
		{
			yield return instance;
		}
		yield break;
	}

	public int IndexOf(Transform item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, Transform item)
	{
		throw new NotImplementedException();
	}

	public void RemoveAt(int index)
	{
		throw new NotImplementedException();
	}

	public void Clear()
	{
		throw new NotImplementedException();
	}

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	bool ICollection<Transform>.Remove(Transform item)
	{
		throw new NotImplementedException();
	}

	public string poolName = string.Empty;

	public bool matchPoolScale;

	public bool matchPoolLayer;

	public bool dontDestroyOnLoad;

	public bool logMessages;

	public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

	public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

	[HideInInspector]
	public float maxParticleDespawnTime = 60f;

	public PrefabsDict prefabs = new PrefabsDict();

	public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

	private BetterList<PrefabPool> _prefabPools = new BetterList<PrefabPool>();

	internal BetterList<Transform> _spawned = new BetterList<Transform>();
}
