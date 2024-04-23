using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.MapSystem
{
	public class MapSpline : MapComponent
	{
		public List<Vector2> Points
		{
			get
			{
				this.isDirty = true;
				return this.points;
			}
		}

		public Vector2 GetPointOnSpline(float t)
		{
			return new Vector2(this.Interpolate(t, this.points[0].x, this.points[1].x, this.points[2].x, this.points[3].x), this.Interpolate(t, this.points[0].y, this.points[1].y, this.points[2].y, this.points[3].y));
		}

		public Vector2 GetEvenlySpacedPosition(float t)
		{
			this.CalculateIfDirty();
			float t2 = this.EuclieanToSpline(t);
			return this.GetPointOnSpline(t2);
		}

		private float EuclieanToSpline(float t)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			for (int i = 0; i < MapSpline.bezierLengths.Length - 1; i++)
			{
				if (t < MapSpline.bezierLengths[i + 1])
				{
					return Mathf.Lerp((float)i / (float)(MapSpline.bezierLengths.Length - 1), (float)(i + 1) / (float)(MapSpline.bezierLengths.Length - 1), (t - MapSpline.bezierLengths[i]) / (MapSpline.bezierLengths[i + 1] - MapSpline.bezierLengths[i]));
				}
			}
			return 1f;
		}

		private void CalculateIfDirty()
		{
			if (!this.isDirty)
			{
				return;
			}
			this.isDirty = false;
			this.CalculateBezierLengths();
		}

		private void CalculateBezierLengths()
		{
			MapSpline.bezierLengths[0] = 0f;
			float num = 0f;
			Vector2 b = this.points[0];
			for (int i = 1; i < MapSpline.bezierLengths.Length; i++)
			{
				Vector2 pointOnSpline = this.GetPointOnSpline((float)i / (float)(MapSpline.bezierLengths.Length - 1));
				num += Vector2.Distance(pointOnSpline, b);
				MapSpline.bezierLengths[i] = num;
				b = pointOnSpline;
			}
			for (int j = 0; j < MapSpline.bezierLengths.Length; j++)
			{
				MapSpline.bezierLengths[j] /= num;
			}
		}

		private float Interpolate(float mu, float x0, float x1, float tangent0, float tangent1)
		{
			return x0 * (1f - mu) * (1f - mu) * (1f - mu) + 3f * tangent0 * ((1f - mu) * (1f - mu)) * mu + 3f * tangent1 * (1f - mu) * mu * mu + x1 * (mu * mu * mu);
		}

		public void InvokeSplineChanged()
		{
			MapSpline.responders.Clear();
			base.GetComponents<IMapSplineResponder>(MapSpline.responders);
			for (int i = 0; i < MapSpline.responders.Count; i++)
			{
				MapSpline.responders[i].SplineChanged(this);
			}
		}

		private static readonly List<IMapSplineResponder> responders = new List<IMapSplineResponder>();

		private static readonly float[] bezierLengths = new float[16];

		[SerializeField]
		private List<Vector2> points;

		private bool isDirty;
	}
}
