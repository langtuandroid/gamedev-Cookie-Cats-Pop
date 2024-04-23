using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapAreas
	{
		public MapAreas(int mapLayer)
		{
			this.mapLayer = mapLayer;
			this.areas = new List<Transform>();
			this.LoadAreas();
		}

		public void Destroy()
		{
			foreach (MapComponent mapComponent in this.IterateComponents<MapComponent>())
			{
				mapComponent.Destroy();
			}
			for (int i = 0; i < this.areas.Count; i++)
			{
				UnityEngine.Object.Destroy(this.areas[i].gameObject);
			}
			this.areas.Clear();
		}

		private void LoadAreas()
		{
			this.areas = new List<Transform>();
			foreach (GameObject areaPrefab in GardenGameSetup.Get.IterateAreaPrefabs(false, true))
			{
				this.areas.Add(this.InstantiateArea(areaPrefab));
			}
		}

		private Transform InstantiateArea(GameObject areaPrefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(areaPrefab);
			gameObject.SetLayerRecursively(this.mapLayer);
			return gameObject.transform;
		}

		public IEnumerable<T> IterateComponents<T>() where T : Component
		{
			for (int i = 0; i < this.areas.Count; i++)
			{
				Transform area = this.areas[i];
				T[] components = area.GetComponentsInChildren<T>(true);
				for (int j = 0; j < components.Length; j++)
				{
					yield return components[j];
				}
			}
			yield break;
		}

		public bool Visible
		{
			get
			{
				return this.areas.Count <= 0 || this.areas[0].gameObject.activeSelf;
			}
			set
			{
				for (int i = 0; i < this.areas.Count; i++)
				{
					this.areas[i].gameObject.SetActive(value);
				}
			}
		}

		private List<Transform> areas;

		private int mapLayer;
	}
}
