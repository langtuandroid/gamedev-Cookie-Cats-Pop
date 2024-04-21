using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool
{
	protected EffectPool()
	{
		this.poolObject = new GameObject("[EffectPool]");
		this.pool = PoolManager.Pools.Create("Effects", this.poolObject);
		this.spawnedEffectsCounter = 0;
		foreach (SpawnedEffect spawnedEffect in this.EnumerateAllEffects())
		{
			PrefabPool prefabPool = new PrefabPool(spawnedEffect.GetComponent<Transform>());
			prefabPool.preloadAmount = spawnedEffect.PrewarmAmount;
			this.pool.CreatePrefabPool(prefabPool);
			this.nameToPrefab.Add(spawnedEffect.gameObject.name, spawnedEffect.GetComponent<Transform>());
		}
	}

	public static EffectPool Instance { get; private set; }

	public GameObject PoolObject
	{
		get
		{
			return this.poolObject;
		}
	}

	public static EffectPool CreateInstance()
	{
		EffectPool.Instance = new EffectPool();
		return EffectPool.Instance;
	}

	private IEnumerable<SpawnedEffect> EnumerateAllEffects()
	{
		foreach (SpawnedEffect e in Resources.LoadAll<SpawnedEffect>(SingletonAsset<EffectPoolSettings>.Instance.EffectPrefabFolderResourcePath))
		{
			yield return e;
		}
		yield break;
	}

	public SpawnedEffect SpawnEffect(string prefabName, Vector3 worldPos, int layer, params object[] parameters)
	{
		string effectPrefabFolderResourcePath = SingletonAsset<EffectPoolSettings>.Instance.EffectPrefabFolderResourcePath;
		Transform prefab = this.nameToPrefab[prefabName];
		SpawnedEffect spawnedEffect = this.SpawnEffect(prefab, worldPos, layer, parameters);
		if (this.EffectSpawned != null)
		{
			this.EffectSpawned(prefabName, spawnedEffect);
		}
		return spawnedEffect;
	}

	public void ValidateEffectForDestruction(SpawnedEffect effect)
	{
		if (this.poolObject == null)
		{
			return;
		}
		if (SingletonAsset<EffectPoolSettings>.Instance.DirectInstantiation || effect.transform.parent != this.poolObject.transform)
		{
		}
	}

	private SpawnedEffect SpawnEffect(Transform prefab, Vector3 worldPos, int layer, params object[] parameters)
	{
		worldPos.z = SingletonAsset<EffectPoolSettings>.Instance.placementInWorldZ - (float)this.spawnedEffectsCounter * 0.1f;
		Transform transform;
		if (SingletonAsset<EffectPoolSettings>.Instance.DirectInstantiation)
		{
			transform = UnityEngine.Object.Instantiate<Transform>(prefab, worldPos, Quaternion.identity);
			transform.gameObject.SetActive(true);
		}
		else
		{
			transform = this.pool.Spawn(prefab, worldPos, Quaternion.identity);
		}
		transform.name = prefab.name;
		transform.gameObject.SetLayerRecursively(layer);
		SpawnedEffect component = transform.GetComponent<SpawnedEffect>();
		component.StartEffect(parameters);
		if (component.SpawnCountingEnabled)
		{
			this.spawnedEffectsCounter++;
		}
		if (!this.spawnCounterPerName.ContainsKey(prefab.name))
		{
			this.spawnCounterPerName[prefab.name] = 1;
		}
		else
		{
			Dictionary<string, int> dictionary;
			string name;
			(dictionary = this.spawnCounterPerName)[name = prefab.name] = dictionary[name] + 1;
		}
		if (!this.runningEffects.Contains(component))
		{
			this.runningEffects.Add(component);
		}
		return component;
	}

	public void DespawnEffect(Transform obj)
	{
		string name = obj.name;
		if (SingletonAsset<EffectPoolSettings>.Instance.DirectInstantiation)
		{
			obj.SendMessage("OnDespawned", SendMessageOptions.RequireReceiver);
			UnityEngine.Object.Destroy(obj.gameObject);
		}
		else
		{
			Transform prefab = this.pool.GetPrefab(obj);
			if (prefab != null)
			{
				name = prefab.name;
			}
			this.pool.Despawn(obj);
		}
		SpawnedEffect component = obj.GetComponent<SpawnedEffect>();
		if (component.SpawnCountingEnabled)
		{
			this.spawnedEffectsCounter--;
		}
		if (this.spawnCounterPerName.ContainsKey(name))
		{
			Dictionary<string, int> dictionary;
			string key;
			(dictionary = this.spawnCounterPerName)[key = name] = dictionary[key] - 1;
		}
		if (this.runningEffects.Contains(component))
		{
			component.transform.parent = this.PoolObject.transform;
			this.runningEffects.Remove(component);
		}
	}

	public bool AnyEffectsPlaying
	{
		get
		{
			return this.spawnedEffectsCounter > 0;
		}
	}

	public IEnumerator WaitForNoEffectWithNamePlaying(string name)
	{
		if (this.spawnCounterPerName.ContainsKey(name))
		{
			while (this.spawnCounterPerName[name] > 0)
			{
				yield return null;
			}
		}
		yield break;
	}

	public void TerminateAllRunningEffects()
	{
		while (this.runningEffects.Count > 0)
		{
			SpawnedEffect spawnedEffect = this.runningEffects[0];
			this.runningEffects.RemoveAt(0);
			spawnedEffect.Terminate();
			spawnedEffect.transform.parent = this.PoolObject.transform;
		}
	}

	private GameObject poolObject;

	private SpawnPool pool;

	private int spawnedEffectsCounter;

	private Dictionary<string, int> spawnCounterPerName = new Dictionary<string, int>();

	private List<SpawnedEffect> runningEffects = new List<SpawnedEffect>();

	private Dictionary<string, Transform> nameToPrefab = new Dictionary<string, Transform>();

	public Action<string, SpawnedEffect> EffectSpawned;

	public const string BonusPieceCollectedEffect = "BonusPieceCollectedEffect";

	public const string BubblePopSmoke = "BubblePopSmoke";

	public const string BurnEffect = "BurnEffect";

	public const string CatPowerReadyEffect = "CatPowerReadyEffect";

	public const string CollectEffect = "CollectEffect";

	public const string ColorChangeHelicopterEffect = "ColorChangeHelicopterEffect";

	public const string ColorChangePaintEffect = "ColorChangePaintEffect";

	public const string DriftingPoints = "DriftingPoints";

	public const string FinalPowerHitEffect = "FinalPowerHitEffect";

	public const string FlyingInk = "FlyingInk";

	public const string FlyingMoves = "FlyingMoves";

	public const string FlyingPlusMoves = "FlyingPlusMoves";

	public const string FrogHitEffect = "FrogHitEffect";

	public const string HotStreakEffect = "HotStreakEffect";

	public const string IceExplode = "IceExplode";

	public const string InkSplatter = "InkSplatter";

	public const string NinjaStarHitEffect = "NinjaStarHitEffect";

	public const string NinjaStarSingleHitEffect = "NinjaStarSingleHitEffect";

	public const string NoteHitEffect = "NoteHitEffect";

	public const string OneTurnBoosterStampEffect = "OneTurnBoosterStampEffect";

	public const string SavedKittenEffect = "SavedKittenEffect";

	public const string SeagullEffect = "SeagullEffect";

	public const string SharkEatBubbleEffect = "SharkEatBubbleEffect";

	public const string SquidLandEffect = "SquidLandEffect";

	public const string SummaryPoints = "SummaryPoints";

	public const string VignetteEffect = "VignetteEffect";
}
