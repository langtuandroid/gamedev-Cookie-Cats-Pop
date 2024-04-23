using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapImage))]
	public class MapImageRevealAnimation : MapComponent, IMapRenderable, IMapPropBuildAnimatable
	{
		public float RevealAmount
		{
			get
			{
				return this.revealAmount;
			}
			set
			{
				this.revealAmount = value;
				if (this.material != null)
				{
					this.material.SetFloat("_RevealPosition", this.revealAmount);
				}
			}
		}

		public MapImageRevealAnimation.Box SelectedBox { get; set; }

		public MapImage MapImage
		{
			get
			{
				if (this.mapImage == null)
				{
					this.mapImage = base.GetComponent<MapImage>();
				}
				return this.mapImage;
			}
		}

		public Quaternion IsometricRotation
		{
			get
			{
				return Quaternion.Euler(-30f, 0f, 0f) * Quaternion.Euler(0f, 45f, 0f);
			}
		}

		public GameObject RenderObject(MapQuadRenderer renderer)
		{
			if (this.disableImage)
			{
				return null;
			}
			MapImageRevealAnimation.meshBuilder.Clear();
			for (int i = 0; i < this.Boxes.Count; i++)
			{
				if (this.Boxes[i] != null)
				{
					this.Boxes[i].BuildMesh(MapImageRevealAnimation.meshBuilder);
				}
			}
			this.material = new Material(GardenGameSetup.Get.revealEffectMaterial);
			this.material.SetTexture("_MainTex", this.MapImage.Sprite.texture);
			this.material.SetMatrix("_QuadSpace", base.transform.worldToLocalMatrix);
			this.material.SetVector("_QuadSize", this.MapImage.Size);
			Vector2[] uv = this.MapImage.Sprite.uv;
			Vector2 lhs = uv[0];
			Vector2 lhs2 = uv[0];
			for (int j = 1; j < uv.Length; j++)
			{
				lhs = Vector2.Min(lhs, uv[j]);
				lhs2 = Vector2.Max(lhs2, uv[j]);
			}
			this.material.SetVector("_QuadTexCoords", new Vector4(lhs.x, lhs.y, lhs2.x - lhs.x, lhs2.y - lhs.y));
			this.RevealAmount = this.revealAmount;
			Mesh mesh = new Mesh();
			MapImageRevealAnimation.meshBuilder.ApplyToMesh(mesh);
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = this.material;
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.localRotation = this.IsometricRotation;
			return gameObject;
		}

		public void BuildInitialize(bool isTeardown)
		{
			if (isTeardown)
			{
				return;
			}
			this.MapImage.DisableImage = true;
			this.disableImage = false;
			this.RevealAmount = 0f;
			base.GetComponent<MapQuadRenderer>().Render();
		}

		public IEnumerator Build(bool isTeardown)
		{
			if (isTeardown)
			{
				yield break;
			}
			while (this.RevealAmount < 2f)
			{
				this.RevealAmount += Time.deltaTime;
				yield return null;
			}
			this.RevealAmount = 2f;
			this.disableImage = true;
			this.MapImage.DisableImage = false;
			base.GetComponent<MapQuadRenderer>().Render();
			yield break;
		}

		private float revealAmount;

		private bool disableImage = true;

		private static readonly Vector3[] cubeVertices = new Vector3[]
		{
			new Vector3(-0.5f, 0f, -0.5f),
			new Vector3(0.5f, 0f, -0.5f),
			new Vector3(0.5f, 0f, 0.5f),
			new Vector3(-0.5f, 0f, 0.5f),
			new Vector3(-0.5f, 1f, -0.5f),
			new Vector3(0.5f, 1f, -0.5f),
			new Vector3(0.5f, 1f, 0.5f),
			new Vector3(-0.5f, 1f, 0.5f)
		};

		private static readonly Vector3[] transformedCubeVertices = new Vector3[8];

		private static readonly Vector3[] sizeHandlePositions = new Vector3[]
		{
			new Vector3(-0.5f, 0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(0f, 0.5f, -0.5f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, 1f, 0f)
		};

		private static readonly Vector3[] transformedSizeHandlePositions = new Vector3[5];

		private static readonly Vector3[] sizeHandleDirections = new Vector3[]
		{
			new Vector3(-1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 1f, 0f)
		};

		private static readonly Vector3[] transformedSizeHandleDirections = new Vector3[5];

		private static readonly MeshBuilder meshBuilder = new MeshBuilder();

		private MaterialPropertyBlock materialPropertyBlock;

		private MapImage mapImage;

		private Material material;

		public List<MapImageRevealAnimation.Box> Boxes = new List<MapImageRevealAnimation.Box>();

		[Serializable]
		public class Box
		{
			public Box()
			{
				this.scale = Vector3.one * 100f;
				this.rotation = Quaternion.identity;
			}

			public Matrix4x4 World
			{
				get
				{
					return Matrix4x4.TRS(this.position, this.rotation, this.scale);
				}
			}

			public Vector3[] GetWorldSpaceVertices()
			{
				Matrix4x4 world = this.World;
				for (int i = 0; i < MapImageRevealAnimation.cubeVertices.Length; i++)
				{
					MapImageRevealAnimation.transformedCubeVertices[i] = world.MultiplyPoint(MapImageRevealAnimation.cubeVertices[i]);
				}
				return MapImageRevealAnimation.transformedCubeVertices;
			}

			public Vector3[] GetWorldSpaceSizeHandlePositions()
			{
				Matrix4x4 world = this.World;
				for (int i = 0; i < MapImageRevealAnimation.sizeHandlePositions.Length; i++)
				{
					MapImageRevealAnimation.transformedSizeHandlePositions[i] = world.MultiplyPoint(MapImageRevealAnimation.sizeHandlePositions[i]);
				}
				return MapImageRevealAnimation.transformedSizeHandlePositions;
			}

			public Vector3[] GetWorldSpaceSizeHandleDirections()
			{
				Matrix4x4 world = this.World;
				for (int i = 0; i < MapImageRevealAnimation.sizeHandleDirections.Length; i++)
				{
					MapImageRevealAnimation.transformedSizeHandleDirections[i] = world.MultiplyVector(MapImageRevealAnimation.sizeHandleDirections[i]);
				}
				return MapImageRevealAnimation.transformedSizeHandleDirections;
			}

			public void BuildMesh(MeshBuilder meshBuilder)
			{
				Vector3[] worldSpaceVertices = this.GetWorldSpaceVertices();
				int count = meshBuilder.vertices.Count;
				meshBuilder.vertices.Add(worldSpaceVertices[0]);
				meshBuilder.vertices.Add(worldSpaceVertices[1]);
				meshBuilder.vertices.Add(worldSpaceVertices[2]);
				meshBuilder.vertices.Add(worldSpaceVertices[4]);
				meshBuilder.vertices.Add(worldSpaceVertices[5]);
				meshBuilder.vertices.Add(worldSpaceVertices[6]);
				meshBuilder.vertices.Add(worldSpaceVertices[7]);
				for (int i = 0; i < 7; i++)
				{
					meshBuilder.colors.Add(Color.white);
				}
				for (int j = 0; j < 7; j++)
				{
					meshBuilder.uvs.Add(new Vector2(0f, this.scale.y));
				}
				meshBuilder.triangles.Add(count);
				meshBuilder.triangles.Add(count + 3);
				meshBuilder.triangles.Add(count + 4);
				meshBuilder.triangles.Add(count);
				meshBuilder.triangles.Add(count + 4);
				meshBuilder.triangles.Add(count + 1);
				meshBuilder.triangles.Add(count + 1);
				meshBuilder.triangles.Add(count + 4);
				meshBuilder.triangles.Add(count + 5);
				meshBuilder.triangles.Add(count + 1);
				meshBuilder.triangles.Add(count + 5);
				meshBuilder.triangles.Add(count + 2);
				meshBuilder.triangles.Add(count + 3);
				meshBuilder.triangles.Add(count + 6);
				meshBuilder.triangles.Add(count + 4);
				meshBuilder.triangles.Add(count + 4);
				meshBuilder.triangles.Add(count + 6);
				meshBuilder.triangles.Add(count + 5);
			}

			public Vector3 position;

			public Quaternion rotation;

			public Vector3 scale;
		}
	}
}
