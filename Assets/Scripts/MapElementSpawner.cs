using System;
using System.Collections;
using Fibers;
using Spine;
using UnityEngine;

public class MapElementSpawner
{
	public void Spawn(MapStreamer mapPanel, MapElementData data)
	{
		this.mapPanel = mapPanel;
		this.data = data;
		if (data.usingAssetBundle)
		{
			this.loadFiber = new Fiber();
			this.loadFiber.Start(this.LoadPrefabFromAssetBundle());
		}
		else
		{
			Transform localPrefabFromName = mapPanel.GetLocalPrefabFromName(data.prefabName);
			if (localPrefabFromName != null)
			{
				this.SpawnFromAsset(localPrefabFromName);
			}
		}
	}

	private void SpawnFromAsset(Transform prefab)
	{
		this.spawnedObject = this.mapPanel.SpawnPool.Spawn(prefab);
		this.PrepareObject(this.spawnedObject);
	}

	private void InstantiateFromAsset(Transform prefab)
	{
		SkeletonJson.LoadSkeletonDataUsingCaching = true;
		this.spawnedObject = UnityEngine.Object.Instantiate<Transform>(prefab);
		SkeletonJson.LoadSkeletonDataUsingCaching = false;
		this.PrepareObject(this.spawnedObject);
	}

	private void PrepareObject(Transform prefab)
	{
		if (this.mapPanel == null || this.spawnedObject == null)
		{
			return;
		}
		this.spawnedObject.parent = this.mapPanel.SpawnedContentRoot;
		this.spawnedObject.localPosition = this.data.position;
		this.spawnedObject.localRotation = this.data.rotation;
		this.spawnedObject.localScale = this.data.scale;
		this.spawnedObject.gameObject.SetLayerRecursively(this.mapPanel.gameObject.layer);
		Hashtable hashtable = this.data.instantiatorDataAsJSON.hashtableFromJson();
		foreach (Instantiator.ComponentPropertyInfo componentPropertyInfo in Instantiator.GetSerializedProperties(this.spawnedObject.gameObject))
		{
			if (hashtable != null && hashtable.ContainsKey(componentPropertyInfo.propertyInfo.Name) && hashtable[componentPropertyInfo.propertyInfo.Name] != null)
			{
				JsonSerializer.SetPropertyFromTableEntry(componentPropertyInfo.component, componentPropertyInfo.propertyInfo, hashtable[componentPropertyInfo.propertyInfo.Name].ToString());
			}
		}
	}

	private IEnumerator LoadPrefabFromAssetBundle()
	{
		GameObject prefab = null;
		string fullName = "assetBundleElement_" + this.data.prefabName;
		yield return MapAssetBundleManager.Instance.DownloadAssetBundleElement(fullName, delegate(object error, AssetBundle temp)
		{
			if (temp != null)
			{
				prefab = temp.LoadAsset<GameObject>(this.data.prefabName);
				if (prefab != null)
				{
					this.InstantiateFromAsset(prefab.transform);
					MapAssetBundleManager.Instance.AcquireAssetBundle(fullName);
				}
			}
		});
		yield break;
	}

	public void BecameVisible()
	{
	}

	public void BecameInvisible()
	{
	}

	public void Despawn()
	{
		Fiber.TerminateIfAble(this.loadFiber);
		if (this.spawnedObject == null)
		{
			return;
		}
		if (this.data.usingAssetBundle)
		{
			if (this.spawnedObject != null)
			{
				UnityEngine.Object.Destroy(this.spawnedObject.gameObject);
				string prefabName = "assetBundleElement_" + this.data.prefabName;
				MapAssetBundleManager.Instance.ReleaseAssetBundle(prefabName);
			}
		}
		else if (this.spawnedObject != null)
		{
			this.mapPanel.SpawnPool.Despawn(this.spawnedObject);
		}
	}

	private Transform spawnedObject;

	private MapStreamer mapPanel;

	private MapElementData data;

	private Fiber loadFiber;
}
