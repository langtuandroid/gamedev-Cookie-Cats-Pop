using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapQuadRenderer))]
	[RequireComponent(typeof(MapPolygon))]
	[RequireComponent(typeof(MapImageCollection))]
	public class MapAreaImage : MapComponent, IMapPolygonResponder, IMapRenderable
	{
		private MapPolygon MapPolygon
		{
			get
			{
				if (this.mapPolygon == null)
				{
					this.mapPolygon = base.GetComponent<MapPolygon>();
				}
				return this.mapPolygon;
			}
		}

		private MapImageCollection MapImageCollection
		{
			get
			{
				if (this.mapImageCollection == null)
				{
					this.mapImageCollection = base.GetComponent<MapImageCollection>();
				}
				return this.mapImageCollection;
			}
		}

		private void GetAreaMinMax(out Vector2 min, out Vector2 max)
		{
			List<Vector3> vertices = this.MapPolygon.Vertices;
			if (vertices.Count == 0)
			{
				min = (max = Vector2.zero);
				return;
			}
			min = (max = vertices[0]);
			for (int i = 1; i < vertices.Count; i++)
			{
				min = Vector2.Min(min, vertices[i]);
				max = Vector2.Max(max, vertices[i]);
			}
		}

		public void PolygonChanged(MapPolygon mapPolygon)
		{
			base.GetComponent<MapQuadRenderer>().Render();
		}

		public GameObject RenderObject(MapQuadRenderer renderer)
		{
			Vector2 b;
			Vector2 a;
			this.GetAreaMinMax(out b, out a);
			Vector2 vector = a - b;
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform, false);
			if (this.MapImageCollection.Images.Count > 0)
			{
				System.Random random = new System.Random(this.Seed);
				int num = Mathf.CeilToInt(vector.x / this.Density);
				int num2 = Mathf.CeilToInt(vector.y / this.Density);
				float num3 = vector.x / (float)num;
				float num4 = vector.y / (float)num2;
				if (num * num2 < 500)
				{
					for (int i = 0; i < num2; i++)
					{
						for (int j = 0; j < num; j++)
						{
							Vector2 vector2 = new Vector2(b.x + (float)j * num3 + num3 * 0.5f + (-1f + 2f * (float)random.NextDouble()) * num3 * 0.5f * this.Irregularity, b.y + (float)i * num4 + num4 * 0.5f + (-1f + 2f * (float)random.NextDouble()) * num4 * 0.5f * this.Irregularity);
							if (this.MapPolygon.Contains(vector2))
							{
								MapImageCollection.Image randomImage = this.MapImageCollection.GetRandomImage(random);
								if (randomImage != null)
								{
									randomImage.CreateChild(vector2, gameObject.transform, random, renderer);
								}
							}
						}
					}
				}
			}
			return gameObject;
		}

		public float Density;

		public int Seed;

		public float Irregularity = 1f;

		private MapPolygon mapPolygon;

		private MapImageCollection mapImageCollection;
	}
}
