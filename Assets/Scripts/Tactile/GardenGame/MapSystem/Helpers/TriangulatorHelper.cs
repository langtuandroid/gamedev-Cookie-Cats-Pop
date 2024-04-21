using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem.Helpers
{
	public static class TriangulatorHelper
	{
		public static void Triangulate(List<Vector3> vertices, List<int> indicies)
		{
			TriangulatorHelper.triangulator.Triangulate(vertices, indicies);
		}

		private static readonly TriangulatorHelper.Triangulator triangulator = new TriangulatorHelper.Triangulator();

		private class Triangulator
		{
			public void Triangulate(List<Vector3> vertices, List<int> indices)
			{
				this.vertices = vertices;
				int count = vertices.Count;
				if (count < 3)
				{
					return;
				}
				int[] array = new int[count];
				if (this.Area() > 0f)
				{
					for (int i = 0; i < count; i++)
					{
						array[i] = i;
					}
				}
				else
				{
					for (int j = 0; j < count; j++)
					{
						array[j] = count - 1 - j;
					}
				}
				int k = count;
				int num = 2 * k;
				int num2 = 0;
				int num3 = k - 1;
				while (k > 2)
				{
					if (num-- <= 0)
					{
						return;
					}
					int num4 = num3;
					if (k <= num4)
					{
						num4 = 0;
					}
					num3 = num4 + 1;
					if (k <= num3)
					{
						num3 = 0;
					}
					int num5 = num3 + 1;
					if (k <= num5)
					{
						num5 = 0;
					}
					if (this.Snip(num4, num3, num5, k, array))
					{
						int item = array[num4];
						int item2 = array[num3];
						int item3 = array[num5];
						indices.Add(item);
						indices.Add(item2);
						indices.Add(item3);
						num2++;
						int num6 = num3;
						for (int l = num3 + 1; l < k; l++)
						{
							array[num6] = array[l];
							num6++;
						}
						k--;
						num = 2 * k;
					}
				}
				indices.Reverse();
			}

			private float Area()
			{
				int count = this.vertices.Count;
				float num = 0f;
				int index = count - 1;
				int i = 0;
				while (i < count)
				{
					Vector2 vector = this.vertices[index];
					Vector2 vector2 = this.vertices[i];
					num += vector.x * vector2.y - vector2.x * vector.y;
					index = i++;
				}
				return num * 0.5f;
			}

			private bool Snip(int u, int v, int w, int n, int[] V)
			{
				Vector2 a = this.vertices[V[u]];
				Vector2 b = this.vertices[V[v]];
				Vector2 c = this.vertices[V[w]];
				if (Mathf.Epsilon > (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x))
				{
					return false;
				}
				for (int i = 0; i < n; i++)
				{
					if (i != u && i != v && i != w)
					{
						Vector2 p = this.vertices[V[i]];
						if (this.InsideTriangle(a, b, c, p))
						{
							return false;
						}
					}
				}
				return true;
			}

			private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
			{
				float num = C.x - B.x;
				float num2 = C.y - B.y;
				float num3 = A.x - C.x;
				float num4 = A.y - C.y;
				float num5 = B.x - A.x;
				float num6 = B.y - A.y;
				float num7 = P.x - A.x;
				float num8 = P.y - A.y;
				float num9 = P.x - B.x;
				float num10 = P.y - B.y;
				float num11 = P.x - C.x;
				float num12 = P.y - C.y;
				float num13 = num * num10 - num2 * num9;
				float num14 = num5 * num8 - num6 * num7;
				float num15 = num3 * num12 - num4 * num11;
				return num13 >= 0f && num15 >= 0f && num14 >= 0f;
			}

			private List<Vector3> vertices;
		}
	}
}
