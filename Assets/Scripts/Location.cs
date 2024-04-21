using System;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	[Serializable]
	public struct Location
	{
		public Location(Vector2 position)
		{
			this.Position = position;
			this.Rotation = 0f;
		}

		public Location(Vector2 position, float rotation)
		{
			this.Position = position;
			this.Rotation = rotation;
		}

		public Vector2 GetArrowEnd(float distance)
		{
			return this.Position + this.Direction * -distance;
		}

		public Vector2 Direction
		{
			get
			{
				return Quaternion.Euler(0f, 0f, this.Rotation) * new Vector3(-1f, 0f, 0f);
			}
		}

		public Vector2 Position;

		public float Rotation;
	}
}
