using System;
using System.Collections.Generic;
using Tactile.GardenGame.Helpers;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapPolygon : MapComponent
	{
		public List<Vector3> Vertices
		{
			get
			{
				return this.vertices;
			}
			set
			{
				this.vertices = value;
			}
		}

		public bool IsListImmutable { get; set; }

		public int CurrentEditingVertex { get; set; }

		private void OnDrawGizmos()
		{
			if (this.vertices == null)
			{
				return;
			}
			if (this.vertices.Count == 0)
			{
				return;
			}
			Bounds boundingBox = this.GetBoundingBox();
			Gizmos.color = new Color(0f, 0f, 0f, 0f);
			Gizmos.DrawCube(boundingBox.center, boundingBox.size);
		}

		public Bounds GetBoundingBox()
		{
			Vector3 vector2;
			Vector3 vector = vector2 = this.vertices[0];
			for (int i = 1; i < this.vertices.Count; i++)
			{
				vector2 = Vector3.Min(vector2, this.vertices[i]);
				vector = Vector3.Max(vector, this.vertices[i]);
			}
			vector2 = base.transform.TransformPoint(vector2);
			vector = base.transform.TransformPoint(vector);
			Bounds result = default(Bounds);
			result.SetMinMax(vector2, vector);
			return result;
		}

		public void InvokePolygonChanged()
		{
			MapPolygon.responders.Clear();
			base.GetComponents<IMapPolygonResponder>(MapPolygon.responders);
			for (int i = 0; i < MapPolygon.responders.Count; i++)
			{
				MapPolygon.responders[i].PolygonChanged(this);
			}
		}

		protected override void Initialized()
		{
			this.InvokePolygonChanged();
		}

		public bool Contains(Vector2 point)
		{
			bool flag = false;
			int i = 0;
			int index = this.vertices.Count - 1;
			while (i < this.vertices.Count)
			{
				if (this.vertices[i].y > point.y != this.vertices[index].y > point.y && point.x < (this.vertices[index].x - this.vertices[i].x) * (point.y - this.vertices[i].y) / (this.vertices[index].y - this.vertices[i].y) + this.vertices[i].x)
				{
					flag = !flag;
				}
				index = i++;
			}
			return flag;
		}

		public bool IntersectsLine(Vector2 start, Vector2 end)
		{
			for (int i = 0; i < this.vertices.Count - 1; i++)
			{
				Vector2 vector = this.vertices[i];
				Vector2 vector2;
				if (i + 1 < this.vertices.Count)
				{
					vector2 = this.vertices[i + 1];
				}
				else
				{
					vector2 = this.vertices[0];
				}
				vector = base.transform.TransformPoint(vector);
				vector2 = base.transform.TransformPoint(vector2);
				if (PhysicsHelper.FastLineSegmentIntersectionTest(start, end, vector, vector2))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly List<IMapPolygonResponder> responders = new List<IMapPolygonResponder>();

		[SerializeField]
		private List<Vector3> vertices;
	}
}
