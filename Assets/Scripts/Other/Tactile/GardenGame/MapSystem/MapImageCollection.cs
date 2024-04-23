using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapImageCollection : MapComponent
	{
		public MapImageCollection.Image GetRandomImage(System.Random rnd)
		{
			float num = 0f;
			if (this.Images.Count == 0)
			{
				return null;
			}
			for (int i = 0; i < this.Images.Count; i++)
			{
				if (this.Images[i] != null)
				{
					num += this.Images[i].Weight;
				}
			}
			float num2 = (float)rnd.NextDouble() * num;
			float num3 = 0f;
			for (int j = 0; j < this.Images.Count; j++)
			{
				if (this.Images[j] != null)
				{
					num3 += this.Images[j].Weight;
					if (num2 <= num3)
					{
						return this.Images[j];
					}
				}
			}
			return null;
		}

		public List<MapImageCollection.Image> Images = new List<MapImageCollection.Image>();

		[Serializable]
		public class Image
		{
			private Vector2 GetSize(System.Random rnd, out float scale)
			{
				scale = this.RandomSize.x + (this.RandomSize.y - this.RandomSize.x) * (float)rnd.NextDouble();
				return this.Size * scale;
			}

			private float GetRotation(System.Random rnd)
			{
				return this.RandomRotation.x + (this.RandomRotation.y - this.RandomRotation.x) * (float)rnd.NextDouble();
			}

			public void CreateChild(Vector2 position, Transform parent, System.Random rnd, MapQuadRenderer renderer)
			{
				float d;
				Vector2 size = this.GetSize(rnd, out d);
				GameObject gameObject = renderer.CreateQuad(parent, this.Position * d, size, this.Sprite);
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, this.GetRotation(rnd));
				gameObject.transform.localPosition = position;
				if (this.ZSorter)
				{
					gameObject.AddComponent<MapZSorter>().GroundPosition = 0f;
				}
			}

			public Sprite Sprite;

			public Vector2 Size;

			public Vector2 Position;

			public Vector2 RandomSize = new Vector2(1f, 1f);

			public Vector2 RandomRotation = new Vector2(0f, 0f);

			public float Weight = 1f;

			public bool ZSorter = true;
		}
	}
}
