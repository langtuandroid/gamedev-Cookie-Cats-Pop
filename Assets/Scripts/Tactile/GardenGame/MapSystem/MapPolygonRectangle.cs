using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapPolygon))]
	public class MapPolygonRectangle : MapComponent
	{
		public Vector2 GridSize
		{
			get
			{
				return this.gridSize;
			}
			set
			{
				this.gridSize = value;
			}
		}

		[SerializeField]
		private Vector2 gridSize;
	}
}
