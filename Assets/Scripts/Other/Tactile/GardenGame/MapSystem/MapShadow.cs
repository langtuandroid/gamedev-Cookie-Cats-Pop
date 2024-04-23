using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapShadow : MapComponent
	{
		protected override void Initialized()
		{
			if (this.shadowInstance != null)
			{
				UnityEngine.Object.Destroy(this.shadowInstance);
				this.shadowInstance = null;
			}
			this.shadowInstance = this.CreateQuad(base.transform, new Vector2(1.18f, base.GetComponent<MapZSorter>().GroundPosition + 0.41f), this.size * 1.3f, this.texture, false);
			this.shadowInstance.SetLayerRecursively(base.gameObject.layer);
		}

		private GameObject CreateQuad(Transform parent, Vector2 quadPosition, Vector2 quadSize, Texture texture, bool flipU = false)
		{
			Vector2 vector = quadSize * 0.5f;
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[]
			{
				new Vector3(quadPosition.x - vector.x, quadPosition.y + vector.y, 0f),
				new Vector3(quadPosition.x + vector.x, quadPosition.y + vector.y, 0f),
				new Vector3(quadPosition.x + vector.x, quadPosition.y - vector.y, 0f),
				new Vector3(quadPosition.x - vector.x, quadPosition.y - vector.y, 0f)
			};
			mesh.triangles = new int[]
			{
				0,
				1,
				2,
				0,
				2,
				3
			};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f)
			};
			Mesh mesh2 = mesh;
			Vector2[] uv;
			if (!flipU)
			{
				Vector2[] array = new Vector2[4];
				array[0] = new Vector2(0f, 1f);
				array[1] = new Vector2(1f, 1f);
				array[2] = new Vector2(1f, 0f);
				uv = array;
				array[3] = new Vector2(0f, 0f);
			}
			else
			{
				Vector2[] array2 = new Vector2[4];
				array2[0] = new Vector2(1f, 1f);
				array2[1] = new Vector2(0f, 1f);
				array2[2] = new Vector2(0f, 0f);
				uv = array2;
				array2[3] = new Vector2(1f, 0f);
			}
			mesh2.uv = uv;
			GameObject gameObject = new GameObject("_instance");
			gameObject.transform.SetParent(parent, false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			Material material = new Material(Shader.Find("Map/Prop"));
			material.mainTexture = texture;
			material.EnableKeyword("ALPHA_SPLITTING_OFF");
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = material;
			meshRenderer.sortingOrder = -32768;
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			return gameObject;
		}

		public Texture2D texture;

		public Vector2 size;

		private GameObject shadowInstance;
	}
}
