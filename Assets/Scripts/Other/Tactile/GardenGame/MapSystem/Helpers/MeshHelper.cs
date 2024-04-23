using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Helpers
{
	public static class MeshHelper
	{
		public static bool MeshContainsWorldPoint(Mesh mesh, Transform meshTransform, Vector3 worldPoint)
		{
			Vector3 v = meshTransform.InverseTransformPoint(worldPoint);
			mesh.GetVertices(MeshHelper.verticesList);
			mesh.GetTriangles(MeshHelper.trianglesList, 0);
			return MeshHelper.MeshContainsPoint(v, MeshHelper.verticesList, MeshHelper.trianglesList);
		}

		public static bool MeshContainsWorldPoint(Vector2[] vertices, ushort[] triangles, Transform meshTransform, Vector3 worldPoint)
		{
			Vector3 v = meshTransform.InverseTransformPoint(worldPoint);
			return MeshHelper.MeshContainsPoint(v, vertices, triangles);
		}

		private static bool MeshContainsPoint(Vector2 point, List<Vector3> vertices, List<int> triangles)
		{
			for (int i = 0; i < triangles.Count; i += 3)
			{
				bool flag = false;
				int j = 0;
				int num = 2;
				while (j < 3)
				{
					int index = triangles[i + j];
					int index2 = triangles[i + num];
					if (vertices[index].y > point.y != vertices[index2].y > point.y && point.x < (vertices[index2].x - vertices[index].x) * (point.y - vertices[index].y) / (vertices[index2].y - vertices[index].y) + vertices[index].x)
					{
						flag = !flag;
					}
					num = j++;
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private static bool MeshContainsPoint(Vector2 point, Vector2[] vertices, ushort[] triangles)
		{
			for (int i = 0; i < triangles.Length; i += 3)
			{
				bool flag = false;
				int j = 0;
				int num = 2;
				while (j < 3)
				{
					int num2 = (int)triangles[i + j];
					int num3 = (int)triangles[i + num];
					if (vertices[num2].y > point.y != vertices[num3].y > point.y && point.x < (vertices[num3].x - vertices[num2].x) * (point.y - vertices[num2].y) / (vertices[num3].y - vertices[num2].y) + vertices[num2].x)
					{
						flag = !flag;
					}
					num = j++;
				}
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private static readonly List<Vector3> verticesList = new List<Vector3>();

		private static readonly List<int> trianglesList = new List<int>();
	}
}
