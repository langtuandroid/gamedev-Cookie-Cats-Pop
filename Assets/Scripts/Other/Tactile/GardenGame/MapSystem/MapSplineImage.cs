using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapQuadRenderer))]
	[RequireComponent(typeof(MapSpline))]
	[RequireComponent(typeof(MapImageCollection))]
	public class MapSplineImage : MapComponent, IMapSplineResponder, IMapRenderable
	{
		private MapSpline MapSpline
		{
			get
			{
				if (this.mapSpline == null)
				{
					this.mapSpline = base.GetComponent<MapSpline>();
				}
				return this.mapSpline;
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

		public void SplineChanged(MapSpline mapSpline)
		{
			base.GetComponent<MapQuadRenderer>().Render();
		}

		public GameObject RenderObject(MapQuadRenderer renderer)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(base.transform, false);
			if (this.MapImageCollection.Images.Count > 0 && this.Count > 1)
			{
				System.Random rnd = new System.Random(this.Seed);
				float num = 1f / (float)(this.Count - 1);
				for (int i = 0; i < this.Count; i++)
				{
					MapImageCollection.Image randomImage = this.MapImageCollection.GetRandomImage(rnd);
					if (randomImage != null)
					{
						Vector2 evenlySpacedPosition = this.MapSpline.GetEvenlySpacedPosition((float)i * num);
						randomImage.CreateChild(evenlySpacedPosition, gameObject.transform, rnd, renderer);
					}
				}
			}
			return gameObject;
		}

		public int Count = 10;

		public int Seed;

		private MapSpline mapSpline;

		private MapImageCollection mapImageCollection;
	}
}
