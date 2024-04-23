using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapPolygon))]
	public class MapCollision : MapComponent
	{
		public static bool RayCast(Vector2 start, Vector2 end)
		{
			MapCollision[] array = UnityEngine.Object.FindObjectsOfType<MapCollision>();
			foreach (MapCollision mapCollision in array)
			{
				if (mapCollision.LineHitTest(start, end))
				{
					return true;
				}
			}
			return false;
		}

		public bool LineHitTest(Vector2 start, Vector2 end)
		{
			MapPolygon component = base.GetComponent<MapPolygon>();
			return !(component == null) && component.IntersectsLine(start, end);
		}

		public Mesh GetExtrudedOutlineMesh(float height, Vector2 scale)
		{
			MapPolygon component = base.GetComponent<MapPolygon>();
			if (component == null)
			{
				return null;
			}
			List<Vector3> vertices = component.Vertices;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector3 vector = vertices[i];
				list.Add(new Vector3(vector.x * scale.x, height, vector.y * scale.y));
			}
			for (int j = 0; j < vertices.Count; j++)
			{
				Vector3 vector2 = vertices[j];
				list.Add(new Vector3(vector2.x * scale.x, -height, vector2.y * scale.y));
			}
			List<int> list2 = new List<int>();
			for (int k = 0; k < vertices.Count; k++)
			{
				int num = k;
				int num2 = (k + 1) % vertices.Count;
				int item = num + vertices.Count;
				int item2 = num2 + vertices.Count;
				list2.Add(num);
				list2.Add(item);
				list2.Add(item2);
				list2.Add(num);
				list2.Add(item2);
				list2.Add(num2);
			}
			Mesh mesh = new Mesh();
			mesh.vertices = list.ToArray();
			mesh.triangles = list2.ToArray();
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static bool IsPathValid(List<Vector2> path)
		{
			if (path == null || path.Count < 0)
			{
				return false;
			}
			for (int i = 0; i < path.Count - 1; i++)
			{
				if (MapCollision.RayCast(path[i], path[i + 1]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
