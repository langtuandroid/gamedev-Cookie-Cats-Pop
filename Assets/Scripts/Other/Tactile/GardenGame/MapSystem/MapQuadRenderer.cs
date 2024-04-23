using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapQuadRenderer : MapComponent
	{
		public void RegisterRenderFunction(MapQuadRenderer.RenderFunction renderFunction)
		{
			this.renderFunctions.Add(renderFunction);
		}

		public override int IntializationOrder
		{
			get
			{
				return 1000;
			}
		}

		public GameObject CreateQuad(Transform parent, Vector2 quadPosition, Vector2 quadSize, Sprite sprite)
		{
			return this.CreateQuad(parent, quadPosition, quadSize, sprite, false, false, Color.white, null);
		}

		public static Color ColorFromFloat(float v)
		{
			Vector4 a = new Vector4(1f, 255f, 65025f, 16581375f);
			float num = 0.003921569f;
			Vector4 vector = a * v;
			vector.x -= Mathf.Floor(vector.x);
			vector.y -= Mathf.Floor(vector.y);
			vector.z -= Mathf.Floor(vector.z);
			vector.w -= Mathf.Floor(vector.w);
			Color result = new Color(vector.x - vector.y * num, vector.y - vector.z * num, vector.w - vector.w * num, vector.z - vector.w * num);
			return result;
		}

		public GameObject CreateQuad(Transform parent, Vector2 quadPosition, Vector2 quadSize, Sprite sprite, bool flipU, bool enableTint, Color tintColor, Material material = null)
		{
			if (sprite == null)
			{
				return null;
			}
			Vector2 vector = quadSize * 0.5f;
			Mesh mesh = new Mesh();
			Vector2[] vertices = sprite.vertices;
			ushort[] triangles = sprite.triangles;
			Vector3[] array = new Vector3[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				array[i] = new Vector3((!flipU) ? vertices[i].x : (-vertices[i].x), vertices[i].y, 0f);
			}
			int[] array2 = new int[triangles.Length];
			for (int j = 0; j < triangles.Length; j++)
			{
				array2[j] = (int)triangles[j];
			}
			mesh.vertices = array;
			mesh.triangles = array2;
			MapZSorter component = base.GetComponent<MapZSorter>();
			Color[] array3 = new Color[array.Length];
			float num = (!(component != null)) ? parent.position.y : component.transform.TransformPoint(new Vector3(0f, component.GroundPosition, 0f)).y;
			num = (num + 50000f) / 100000f;
			Color color = MapQuadRenderer.ColorFromFloat(num);
			for (int k = 0; k < array3.Length; k++)
			{
				array3[k] = color;
			}
			mesh.colors = array3;
			if (MapQuadRenderer.materialPropertyBlock == null)
			{
				MapQuadRenderer.materialPropertyBlock = new MaterialPropertyBlock();
			}
			GameObject gameObject = new GameObject("_instance");
			gameObject.transform.SetParent(parent, false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			if (enableTint)
			{
				meshRenderer.material = ((!(material == null)) ? material : GardenGameSetup.Get.propTintedMaterial);
				MapQuadRenderer.materialPropertyBlock.SetColor("_Color", tintColor);
			}
			else
			{
				meshRenderer.material = ((!(material == null)) ? material : GardenGameSetup.Get.propMaterial);
			}
			mesh.uv = sprite.uv;
			MapQuadRenderer.materialPropertyBlock.SetTexture("_MainTex", sprite.texture);
			if (sprite.associatedAlphaSplitTexture != null)
			{
				MapQuadRenderer.materialPropertyBlock.SetTexture("_AlphaTex", sprite.associatedAlphaSplitTexture);
			}
			meshRenderer.SetPropertyBlock(MapQuadRenderer.materialPropertyBlock);
			return gameObject;
		}

		private void DeleteAllRenderedObjects()
		{
			foreach (GameObject gameObject in this.renderedObjects)
			{
				if (gameObject != null)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(gameObject);
					}
				}
			}
			this.renderedObjects.Clear();
		}

		public void Render()
		{
			this.DeleteAllRenderedObjects();
			MapQuadRenderer.mapRenderables.Clear();
			base.GetComponents<IMapRenderable>(MapQuadRenderer.mapRenderables);
			foreach (IMapRenderable mapRenderable in MapQuadRenderer.mapRenderables)
			{
				this.CreateObjectFromRenderable(mapRenderable);
			}
		}

		private void CreateObjectFromRenderable(IMapRenderable mapRenderable)
		{
			GameObject gameObject = mapRenderable.RenderObject(this);
			if (gameObject == null)
			{
				return;
			}
			this.renderedObjects.Add(gameObject);
			gameObject.SetLayerRecursively(base.gameObject.layer);
			if (!Application.isPlaying)
			{
				gameObject.SetHideFlagsRecursively(HideFlags.HideAndDontSave);
			}
			else
			{
				gameObject.SetHideFlagsRecursively(HideFlags.DontSave);
			}
			MapZSorter component = base.gameObject.GetComponent<MapZSorter>();
			if (component != null)
			{
				component.MarkDirty();
			}
		}

		protected override void Initialized()
		{
			this.Render();
		}

		private readonly List<MapQuadRenderer.RenderFunction> renderFunctions = new List<MapQuadRenderer.RenderFunction>();

		private readonly List<GameObject> renderedObjects = new List<GameObject>();

		private static readonly List<IMapRenderable> mapRenderables = new List<IMapRenderable>();

		private static MaterialPropertyBlock materialPropertyBlock;

		public delegate GameObject RenderFunction();
	}
}
