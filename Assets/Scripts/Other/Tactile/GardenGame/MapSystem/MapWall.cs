using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[RequireComponent(typeof(MapZSorter))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class MapWall : MapComponent
	{
		public Vector2 PlaneNormal
		{
			get
			{
				return this.planeNormal;
			}
			set
			{
				this.planeNormal = value;
			}
		}

		public Vector2 Origin()
		{
			return base.transform.TransformPoint(this.planeNormal * this.GroundPosition);
		}

		public bool IsBehindWall(Vector2 point)
		{
			Vector2 normalized = (point - this.Origin()).normalized;
			float num = Vector2.Dot(normalized, this.planeNormal);
			return num >= 0f;
		}

		public float GroundPosition;

		private static readonly List<Renderer> rendererResults = new List<Renderer>();

		[SerializeField]
		private Vector2 planeNormal = Vector2.up;
	}
}
