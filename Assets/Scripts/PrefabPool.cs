using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PrefabPool
{
	public PrefabPool(Transform prefab)
	{
		this.prefab = prefab;
		this.prefabGO = prefab.gameObject;
	}

	public PrefabPool()
	{
	}

	public bool logMessages
	{
		get
		{
			if (this.forceLoggingSilent)
			{
				return false;
			}
			if (this.spawnPool.logMessages)
			{
				return this.spawnPool.logMessages;
			}
			return this._logMessages;
		}
	}

	internal void inspectorInstanceConstructor()
	{
		this.prefabGO = this.prefab.gameObject;
		this._spawned = new BetterList<Transform>();
		this._despawned = new BetterList<Transform>();
	}

	internal void SelfDestruct()
	{
		this.prefab = null;
		this.prefabGO = null;
		this.spawnPool = null;
		foreach (Transform transform in this._despawned)
		{
			if (transform != null)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		foreach (Transform transform2 in this._spawned)
		{
			if (transform2 != null)
			{
				UnityEngine.Object.Destroy(transform2.gameObject);
			}
		}
		this._spawned.Clear();
		this._despawned.Clear();
	}

	public BetterList<Transform> spawned
	{
		get
		{
			return this._spawned;
		}
	}

	public BetterList<Transform> despawned
	{
		get
		{
			return this._despawned;
		}
	}

	public int totalCount
	{
		get
		{
			int num = 0;
			num += this._spawned.size;
			return num + this._despawned.size;
		}
	}

	internal bool preloaded
	{
		get
		{
			return this._preloaded;
		}
		private set
		{
			this._preloaded = value;
		}
	}

	internal bool DespawnInstance(Transform xform)
	{
		if (this.logMessages)
		{
		}
		this._spawned.Remove(xform);
		this._despawned.Add(xform);
		xform.gameObject.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
		PoolManagerUtils.SetActive(xform.gameObject, false);
		if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
		{
			this.cullingActive = true;
			this.spawnPool.StartCoroutine(this.CullDespawned());
		}
		return true;
	}

	internal IEnumerator CullDespawned()
	{
		if (this.logMessages)
		{
		}
		yield return new WaitForSeconds((float)this.cullDelay);
		while (this.totalCount > this.cullAbove)
		{
			for (int i = 0; i < this.cullMaxPerPass; i++)
			{
				if (this.totalCount <= this.cullAbove)
				{
					break;
				}
				if (this._despawned.size > 0)
				{
					Transform transform = this._despawned[0];
					this._despawned.RemoveAt(0);
					UnityEngine.Object.Destroy(transform.gameObject);
					if (this.logMessages)
					{
					}
				}
				else if (this.logMessages)
				{
					break;
				}
			}
			yield return new WaitForSeconds((float)this.cullDelay);
		}
		if (this.logMessages)
		{
		}
		this.cullingActive = false;
		yield return null;
		yield break;
	}

	internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
	{
		if (this.limitInstances && this.limitFIFO && this._spawned.size >= this.limitAmount)
		{
			Transform transform = this._spawned[0];
			if (this.logMessages)
			{
			}
			this.DespawnInstance(transform);
			this.spawnPool._spawned.Remove(transform);
		}
		Transform transform2;
		if (this._despawned.size == 0)
		{
			transform2 = this.SpawnNew(pos, rot);
		}
		else
		{
			transform2 = this._despawned[0];
			this._despawned.RemoveAt(0);
			this._spawned.Add(transform2);
			if (transform2 == null)
			{
				string message = "Make sure you didn't delete a despawned instance directly.";
				throw new MissingReferenceException(message);
			}
			if (this.logMessages)
			{
			}
			transform2.position = pos;
			transform2.rotation = rot;
			PoolManagerUtils.SetActive(transform2.gameObject, true);
		}
		if (transform2 != null)
		{
			transform2.gameObject.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
		}
		return transform2;
	}

	public Transform SpawnNew()
	{
		return this.SpawnNew(Vector3.zero, Quaternion.identity);
	}

	public Transform SpawnNew(Vector3 pos, Quaternion rot)
	{
		if (this.limitInstances && this.totalCount >= this.limitAmount)
		{
			if (this.logMessages)
			{
			}
			return null;
		}
		if (pos == Vector3.zero)
		{
			pos = this.spawnPool.group.position;
		}
		if (rot == Quaternion.identity)
		{
			rot = this.spawnPool.group.rotation;
		}
		Transform transform = UnityEngine.Object.Instantiate<Transform>(this.prefab, pos, rot);
		this.nameInstance(transform);
		transform.parent = this.spawnPool.group;
		if (this.spawnPool.matchPoolScale)
		{
			transform.localScale = Vector3.one;
		}
		if (this.spawnPool.matchPoolLayer)
		{
			this.SetRecursively(transform, this.spawnPool.gameObject.layer);
		}
		this._spawned.Add(transform);
		if (this.logMessages)
		{
		}
		return transform;
	}

	private void SetRecursively(Transform xform, int layer)
	{
		xform.gameObject.layer = layer;
		IEnumerator enumerator = xform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform xform2 = (Transform)obj;
				this.SetRecursively(xform2, layer);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	internal void AddUnpooled(Transform inst, bool despawn)
	{
		this.nameInstance(inst);
		if (despawn)
		{
			PoolManagerUtils.SetActive(inst.gameObject, false);
			this._despawned.Add(inst);
		}
		else
		{
			this._spawned.Add(inst);
		}
	}

	internal void PreloadInstances()
	{
		if (this.preloaded)
		{
			return;
		}
		if (this.prefab == null)
		{
			return;
		}
		if (this.limitInstances && this.preloadAmount > this.limitAmount)
		{
			this.preloadAmount = this.limitAmount;
		}
		if (!this.cullDespawned || this.preloadAmount > this.cullAbove)
		{
		}
		if (this.preloadTime)
		{
			if (this.preloadFrames > this.preloadAmount)
			{
				this.preloadFrames = this.preloadAmount;
			}
			this.spawnPool.StartCoroutine(this.PreloadOverTime());
		}
		else
		{
			this.forceLoggingSilent = true;
			while (this.totalCount < this.preloadAmount)
			{
				Transform xform = this.SpawnNew();
				this.DespawnInstance(xform);
			}
			this.forceLoggingSilent = false;
		}
	}

	private IEnumerator PreloadOverTime()
	{
		yield return new WaitForSeconds(this.preloadDelay);
		int amount = this.preloadAmount - this.totalCount;
		if (amount <= 0)
		{
			yield break;
		}
		int numPerFrame = amount / this.preloadFrames;
		int remainder = amount % this.preloadFrames;
		this.forceLoggingSilent = true;
		for (int i = 0; i < this.preloadFrames; i++)
		{
			int numThisFrame = numPerFrame;
			if (i == this.preloadFrames - 1)
			{
				numThisFrame += remainder;
			}
			for (int j = 0; j < numThisFrame; j++)
			{
				Transform inst = this.SpawnNew();
				if (inst != null)
				{
					this.DespawnInstance(inst);
				}
				yield return null;
			}
			if (this.totalCount >= this.preloadAmount)
			{
				break;
			}
		}
		this.forceLoggingSilent = false;
		yield break;
	}

	private void nameInstance(Transform instance)
	{
		instance.name += (this.totalCount + 1).ToString("#000");
	}

	public Transform prefab;

	internal GameObject prefabGO;

	public int preloadAmount = 1;

	public bool preloadTime;

	public int preloadFrames = 2;

	public float preloadDelay;

	public bool limitInstances;

	public int limitAmount = 100;

	public bool limitFIFO;

	public bool cullDespawned;

	public int cullAbove = 50;

	public int cullDelay = 60;

	public int cullMaxPerPass = 5;

	public bool _logMessages;

	private bool forceLoggingSilent;

	public SpawnPool spawnPool;

	private bool cullingActive;

	internal BetterList<Transform> _spawned = new BetterList<Transform>();

	internal BetterList<Transform> _despawned = new BetterList<Transform>();

	private bool _preloaded;
}
