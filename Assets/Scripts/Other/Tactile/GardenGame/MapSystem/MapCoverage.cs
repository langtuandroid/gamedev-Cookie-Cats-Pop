using System;
using System.Collections.Generic;
using Tactile.GardenGame.MapSystem.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapPolygon))]
	[RequireComponent(typeof(MapQuadRenderer))]
	public class MapCoverage : MapComponent, IMapRenderable, IMapPolygonResponder
	{
		public float MaxHeight
		{
			get
			{
				return this.maxHeight;
			}
			set
			{
				this.maxHeight = value;
				if (this.material != null)
				{
					this.material.SetFloat("_MaxHeight", this.maxHeight);
				}
			}
		}

		public GameObject RenderObject(MapQuadRenderer renderer)
		{
			this.EnsureMesh();
			GameObject gameObject = new GameObject("clouds");
			gameObject.transform.SetParent(base.transform, false);
			gameObject.AddComponent<MeshFilter>().sharedMesh = this.mesh;
			this.material = new Material(GardenGameSetup.Get.coverageMaterial);
			gameObject.AddComponent<MeshRenderer>().material = this.material;
			this.MaxHeight = this.maxHeight;
			if (this.RenderObjectCreated != null)
			{
				this.RenderObjectCreated(gameObject);
			}
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
			MapCoverage.trianglesList.Clear();
			List<Vector3> list = new List<Vector3>();
			TriangulatorHelper.Triangulate(component.Vertices, MapCoverage.trianglesList);
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
				list[i + component.Vertices.Count] = list[i] + a2 * 200f;
			}
			int count = component.Vertices.Count;
			for (int j = 0; j < component.Vertices.Count; j++)
			{
				if (j < component.Vertices.Count - 1)
				{
					MapCoverage.trianglesList.Add(j);
					MapCoverage.trianglesList.Add(j + component.Vertices.Count);
					MapCoverage.trianglesList.Add(j + component.Vertices.Count + 1);
					MapCoverage.trianglesList.Add(j);
					MapCoverage.trianglesList.Add(j + component.Vertices.Count + 1);
					MapCoverage.trianglesList.Add(j + 1);
				}
				else
				{
					MapCoverage.trianglesList.Add(j);
					MapCoverage.trianglesList.Add(j + component.Vertices.Count);
					MapCoverage.trianglesList.Add(component.Vertices.Count);
					MapCoverage.trianglesList.Add(j);
					MapCoverage.trianglesList.Add(component.Vertices.Count);
					MapCoverage.trianglesList.Add(0);
				}
			}
			mesh.vertices = list.ToArray();
			mesh.triangles = MapCoverage.trianglesList.ToArray();
			Color[] array = new Color[list.Count];
			int num = array.Length / 2;
			for (int k = 0; k < num; k++)
			{
				array[k] = Color.white;
			}
			for (int l = num; l < list.Count; l++)
			{
				array[l] = new Color(0f, 0f, 0f, 0f);
			}
			mesh.colors = array;
			Vector2[] array2 = new Vector2[list.Count];
			for (int m = 0; m < array2.Length; m++)
			{
				Vector3 position = list[m];
				position = base.transform.TransformPoint(position);
				array2[m] = new Vector2(position.x * 0.01f, position.y * 0.01f);
			}
			mesh.uv = array2;
			mesh.RecalculateBounds();
		}

		private Mesh mesh;

		private static readonly List<int> trianglesList = new List<int>();

		private Material material;

		public Action<GameObject> RenderObjectCreated;

		[SerializeField]
		private float maxHeight;
	}
}
