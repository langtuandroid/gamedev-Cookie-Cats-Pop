using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapLight : MapComponent
	{
		protected override void Initialized()
		{
			this.CreateQuad();
		}

		private void CreateQuad()
		{
			if (this.lightQuad != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(this.lightQuad);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(this.lightQuad);
				}
			}
			this.lightQuad = this.CreateQuad(base.transform, Vector2.one, this.lightTexture, false);
			this.lightQuad.SetLayerRecursively(GardenGameSetup.Get.lightingLayer);
			if (!Application.isPlaying)
			{
				this.lightQuad.SetHideFlagsRecursively(HideFlags.HideAndDontSave);
			}
		}

		private void LateUpdate()
		{
			if (this.lightQuad != null)
			{
				this.lightQuad.transform.localScale = Vector3.one * this.radius;
			}
		}

		protected GameObject CreateQuad(Transform parent, Vector2 quadSize, Texture texture, bool flipU = false)
		{
			Vector2 vector = quadSize * 0.5f;
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[]
			{
				new Vector3(-vector.x, vector.y, 0f),
				new Vector3(vector.x, vector.y, 0f),
				new Vector3(vector.x, -vector.y, 0f),
				new Vector3(-vector.x, -vector.y, 0f)
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
			this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
			this.meshRenderer.sharedMaterial = GardenGameSetup.Get.lightMaterial;
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			this.Refresh();
			return gameObject;
		}

		public void Refresh()
		{
			if (this.materialPropertyBlock == null)
			{
				this.materialPropertyBlock = new MaterialPropertyBlock();
			}
			this.materialPropertyBlock.SetTexture("_MainTex", this.lightTexture);
			this.materialPropertyBlock.SetColor("_Color", this.color);
			this.meshRenderer.SetPropertyBlock(this.materialPropertyBlock);
		}

		public Texture2D lightTexture;

		public float radius;

		public Color color = Color.white;

		private GameObject lightQuad;

		private MeshRenderer meshRenderer;

		private MaterialPropertyBlock materialPropertyBlock;
	}
}
