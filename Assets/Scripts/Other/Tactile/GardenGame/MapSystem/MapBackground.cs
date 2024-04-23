using System;
using System.Collections.Generic;
using Tactile.GardenGame.MapSystem.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapPolygon))]
	[RequireComponent(typeof(MapQuadRenderer))]
	public class MapBackground : MapComponent, IMapRenderable, IMapPolygonResponder
	{
		public Texture2D Texture
		{
			get
			{
				return this.texture;
			}
			set
			{
				if (this.texture == value)
				{
					return;
				}
				this.texture = value;
				base.GetComponent<MapQuadRenderer>().Render();
				this.RefreshMesh();
			}
		}

		public Vector2 TextureSize
		{
			get
			{
				return this.textureSize;
			}
			set
			{
				if (this.textureSize == value)
				{
					return;
				}
				this.textureSize = value;
				this.RefreshMesh();
			}
		}

		public float EdgeSize
		{
			get
			{
				return this.edgeSize;
			}
			set
			{
				if (this.edgeSize == value)
				{
					return;
				}
				this.edgeSize = value;
				this.RefreshMesh();
			}
		}

		public bool Transparent
		{
			get
			{
				return this.transparent;
			}
			set
			{
				if (this.transparent == value)
				{
					return;
				}
				this.transparent = value;
				base.GetComponent<MapQuadRenderer>().Render();
				this.RefreshMesh();
			}
		}

		private void RefreshMesh()
		{
			this.PolygonChanged(base.GetComponent<MapPolygon>());
		}

		public GameObject RenderObject(MapQuadRenderer renderer)
		{
			this.EnsureMesh();
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform, false);
			gameObject.AddComponent<MeshFilter>().sharedMesh = this.mesh;
			Material material = new Material(Shader.Find((!this.transparent) ? "Map/Tile_editor" : "Map/Prop"));
			material.mainTexture = this.texture;
			gameObject.AddComponent<MeshRenderer>().material = material;
			return gameObject;
		}

		public void PolygonChanged(MapPolygon mapPolygon)
		{
			this.EnsureMesh();
			this.UpdateMesh(this.mesh);
		}

		private void EnsureMesh()
		{
			if (this.mesh != null)
			{
				return;
			}
			this.mesh = new Mesh();
		}

		private void UpdateMesh(Mesh mesh)
		{
			MapPolygon component = base.GetComponent<MapPolygon>();
			MapBackground.trianglesList.Clear();
			List<Vector3> list = new List<Vector3>();
			TriangulatorHelper.Triangulate(component.Vertices, MapBackground.trianglesList);
			list.AddRange(component.Vertices);
			list.AddRange(component.Vertices);
			for (int i = 0; i < component.Vertices.Count; i++)
			{
				Vector3 a = list[i];
				Vector3 b = list[(i != 0) ? (i - 1) : (component.Vertices.Count - 1)];
				Vector3 b2 = list[(i + 1) % component.Vertices.Count];
				Vector3 vector = a - b;
				Vector3 lhs = a - b2;
				Vector3 a2 = Vector3.Normalize(vector.normalized + lhs.normalized);
				if (Vector3.Dot(lhs, new Vector3(vector.y, -vector.x, 0f)) > 0f)
				{
					a2 = -a2;
				}
				list[i + component.Vertices.Count] = list[i] + a2 * this.edgeSize;
			}
			int count = component.Vertices.Count;
			for (int j = 0; j < component.Vertices.Count; j++)
			{
				if (j < component.Vertices.Count - 1)
				{
					MapBackground.trianglesList.Add(j);
					MapBackground.trianglesList.Add(j + component.Vertices.Count);
					MapBackground.trianglesList.Add(j + component.Vertices.Count + 1);
					MapBackground.trianglesList.Add(j);
					MapBackground.trianglesList.Add(j + component.Vertices.Count + 1);
					MapBackground.trianglesList.Add(j + 1);
				}
				else
				{
					MapBackground.trianglesList.Add(j);
					MapBackground.trianglesList.Add(j + component.Vertices.Count);
					MapBackground.trianglesList.Add(component.Vertices.Count);
					MapBackground.trianglesList.Add(j);
					MapBackground.trianglesList.Add(component.Vertices.Count);
					MapBackground.trianglesList.Add(0);
				}
			}
			mesh.vertices = list.ToArray();
			mesh.triangles = MapBackground.trianglesList.ToArray();
			Color[] array = new Color[list.Count];
			int num = array.Length / 2;
			for (int k = 0; k < num; k++)
			{
				array[k] = Color.white;
			}
			for (int l = num; l < list.Count; l++)
			{
				array[l] = new Color(1f, 1f, 1f, 0f);
			}
			mesh.colors = array;
			Vector2[] array2 = new Vector2[list.Count];
			for (int m = 0; m < array2.Length; m++)
			{
				Vector3 position = list[m];
				position = base.transform.TransformPoint(position);
				array2[m] = new Vector2(position.x / this.textureSize.x, position.y / this.textureSize.y);
			}
			mesh.uv = array2;
			mesh.RecalculateBounds();
		}

		[SerializeField]
		private Texture2D texture;

		[SerializeField]
		private Vector2 textureSize = new Vector2(1024f, 1024f);

		[SerializeField]
		private float edgeSize = 10f;

		[SerializeField]
		private bool transparent;

		private Mesh mesh;

		private static readonly List<int> trianglesList = new List<int>();
	}
}
