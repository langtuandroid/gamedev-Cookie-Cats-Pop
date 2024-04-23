using System;
using System.Collections.Generic;
using TactileModules.GardenGame.MapSystem.Assets;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapInformation
	{
		public MapInformation(MapAreas areas)
		{
			this.CollectObjects(areas);
			this.GetWorldBounds(areas);
		}

		public Dictionary<string, MapObjectID> Entities { get; private set; }

		public Bounds GroundBounds { get; private set; }

		public List<MapTile> MapTiles { get; private set; }

		private void CollectObjects(MapAreas areas)
		{
			this.Entities = new Dictionary<string, MapObjectID>();
			Dictionary<string, List<MapObjectID>> dictionary = new Dictionary<string, List<MapObjectID>>();
			foreach (MapObjectID mapObjectID in areas.IterateComponents<MapObjectID>())
			{
				List<MapObjectID> list = new List<MapObjectID>();
				if (!dictionary.TryGetValue(mapObjectID.Id, out list))
				{
					list = new List<MapObjectID>();
					dictionary.Add(mapObjectID.Id, list);
				}
				list.Add(mapObjectID);
			}
			foreach (KeyValuePair<string, List<MapObjectID>> keyValuePair in dictionary)
			{
				if (keyValuePair.Value.Count > 1)
				{
					for (int i = 0; i < keyValuePair.Value.Count; i++)
					{
					}
				}
			}
			foreach (MapObjectID mapObjectID2 in areas.IterateComponents<MapObjectID>())
			{
				if (this.Entities.ContainsKey(mapObjectID2.Id))
				{
					throw new Exception("Duplicate map entity id: " + mapObjectID2.Id);
				}
				this.Entities.Add(mapObjectID2.Id, mapObjectID2);
			}
		}

		private void GetWorldBounds(MapAreas areas)
		{
			Vector2 camLimits = GardenGameSetup.Get.camLimits;
			if (camLimits != Vector2.zero)
			{
				this.GroundBounds = new Bounds(camLimits * 0.5f, camLimits);
			}
			else
			{
				AssetModel assetModel = new AssetModel();
				Vector2 vector = new Vector2((float)assetModel.GetMapTileDatabase().numX, (float)assetModel.GetMapTileDatabase().numY) * 1024f;
				this.GroundBounds = new Bounds(vector * -0.5f, vector);
			}
		}

		public MapObjectID GetEntity(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			MapObjectID result;
			this.Entities.TryGetValue(id, out result);
			return result;
		}

		public string GetEntityIdFromName(string name)
		{
			foreach (KeyValuePair<string, MapObjectID> keyValuePair in this.Entities)
			{
				if (keyValuePair.Value.name == name)
				{
					return keyValuePair.Key;
				}
			}
			return string.Empty;
		}

		public T GetMapComponent<T>(string id) where T : MapComponent
		{
			MapObjectID entity = this.GetEntity(id);
			return (!(entity != null)) ? ((T)((object)null)) : entity.gameObject.GetComponent<T>();
		}

		public T FindObjectWithComponent<T>() where T : MapComponent
		{
			foreach (KeyValuePair<string, MapObjectID> keyValuePair in this.Entities)
			{
				T component = keyValuePair.Value.gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			return (T)((object)null);
		}

		public IEnumerable<T> IterateObjectsWithComponent<T>()
		{
			foreach (KeyValuePair<string, MapObjectID> objectId in this.Entities)
			{
				if (!(objectId.Value == null))
				{
					if (!(objectId.Value.gameObject == null))
					{
						T component = objectId.Value.gameObject.GetComponent<T>();
						if (component != null)
						{
							yield return component;
						}
					}
				}
			}
			yield break;
		}
	}
}
