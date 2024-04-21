using System;
using UnityEngine;

namespace Tactile
{
	public class MaskOverlayCutout
	{
		public Vector2 ViewportPosition
		{
			get
			{
				return this.viewportPosition;
			}
			set
			{
				this.viewportPosition = value;
				if (this.changed != null)
				{
					this.changed();
				}
			}
		}

		public Vector2 ViewportSize
		{
			get
			{
				return this.viewportSize;
			}
			set
			{
				this.viewportSize = value;
				if (this.changed != null)
				{
					this.changed();
				}
			}
		}

		public bool Oval
		{
			get
			{
				return this.oval;
			}
			set
			{
				this.oval = value;
				if (this.changed != null)
				{
					this.changed();
				}
			}
		}

		public Camera Camera { get; set; }

		public void SetFromWorldSpace(Camera camera, Vector3 worldPosition, Vector2 worldSize)
		{
			this.Camera = camera;
			this.WorldPosition = worldPosition;
			this.WorldSize = worldSize;
		}

		public Vector3 WorldPosition
		{
			get
			{
				return this.worldPosition;
			}
			set
			{
				this.worldPosition = value;
				if (this.Camera == null)
				{
					this.viewportPosition = new Vector2(value.x, value.y);
					return;
				}
				Vector3 vector = this.Camera.WorldToViewportPoint(value);
				this.ViewportPosition = new Vector2(vector.x, vector.y);
			}
		}

		public Vector2 WorldSize
		{
			get
			{
				return this.worldSize;
			}
			set
			{
				this.worldSize = value;
				if (this.Camera == null)
				{
					this.viewportSize = value;
					return;
				}
				Vector3 vector = (this.Camera.worldToCameraMatrix * this.Camera.projectionMatrix).MultiplyVector(new Vector3(value.x, value.y, 0f));
				this.ViewportSize = new Vector2(vector.x, vector.y);
			}
		}

		public string Layer { get; set; }

		public void Refresh()
		{
			if (this.Camera == null)
			{
				return;
			}
			this.WorldPosition = this.worldPosition;
			this.WorldSize = this.worldSize;
		}

		private Vector2 viewportPosition;

		private Vector2 viewportSize;

		private bool oval;

		private Vector3 worldPosition;

		private Vector2 worldSize;

		public Action changed;
	}
}
