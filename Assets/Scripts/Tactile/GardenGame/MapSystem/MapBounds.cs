using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapPolygonRectangle))]
	public class MapBounds : MapComponent
	{
		public Bounds GetWorldBounds()
		{
			return base.GetComponent<MapPolygon>().GetBoundingBox();
		}
	}
}
