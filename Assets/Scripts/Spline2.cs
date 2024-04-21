using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Spline2
{
	public void MakeDirty()
	{
		this.isDirty = true;
		if (this.WasMadeDirty != null)
		{
			this.WasMadeDirty(this);
		}
	}

	//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Spline2.DirtyHandler WasMadeDirty;

	public float FullLength
	{
		get
		{
			return this.fullLength;
		}
	}

	public float RealLength
	{
		get
		{
			return this.realLength;
		}
	}

	public float SegmentsLength
	{
		get
		{
			return this.segmentsLength;
		}
	}

	public void CalculateSegments(int segmentsCount)
	{
		float num = this.fullLength / (float)segmentsCount;
		this.segments = new List<Spline2.Segment>();
		this.segmentsLength = 0f;
		float num2 = 0f;
		for (int i = 0; i < segmentsCount; i++)
		{
			Spline2.Segment segment = new Spline2.Segment(this.Evaluate(num2), this.Evaluate(num2 + num), num2, num2 + num);
			this.segmentsLength += segment.length;
			this.segments.Add(segment);
			num2 += num;
		}
	}

	public Vector2 GetLinearPosition(float position)
	{
		float t;
		Spline2.Segment segment = this.FindSegment(position, out t);
		return Vector2.Lerp(segment.from, segment.to, t);
	}

	private Spline2.Segment FindSegment(float position, out float amount)
	{
		if (position < 0f)
		{
			amount = 0f;
			return this.segments[0];
		}
		if (position >= this.segmentsLength)
		{
			Spline2.Segment result = this.segments[this.segments.Count - 1];
			amount = 1f;
			return result;
		}
		amount = 0f;
		float num = 0f;
		for (int i = 0; i < this.segments.Count; i++)
		{
			float length = this.segments[i].length;
			if (position < num + length)
			{
				amount = (position - num) / length;
				return this.segments[i];
			}
			num += length;
		}
		return this.segments[0];
	}

	private int GetIndex(int i)
	{
		while (i < 0)
		{
			i += this.Nodes.Count;
		}
		return i % this.Nodes.Count;
	}

	public void CalculateSpline()
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		int num = this.Cyclic ? this.Nodes.Count : (this.Nodes.Count - 1);
		this.curves = new List<Spline2.Curve>();
		for (int i = 0; i < num; i++)
		{
			this.curves.Add(new Spline2.Curve());
		}
		this.fullLength = 0f;
		if (!this.Cyclic)
		{
			for (int j = 0; j < num; j++)
			{
				Vector2 vector = this.Nodes[j];
				Vector2 vector2 = this.Nodes[j + 1];
				Vector2 a = vector2 - vector;
				Spline2.Curve curve = this.curves[j];
				curve.Length = a.magnitude;
				curve.Start = vector;
				curve.End = vector2;
				this.fullLength += curve.Length;
				if (j == 0)
				{
					curve.StartTangent = curve.Start + a * 0.333333343f;
				}
				else
				{
					Vector2 vector3 = this.Nodes[j] - this.Nodes[j - 1];
					curve.StartTangent = curve.Start + (vector3.normalized + a.normalized).normalized * curve.Length * 0.333333343f;
				}
				if (j == this.Nodes.Count - 2)
				{
					curve.EndTangent = curve.End - a * 0.333333343f;
				}
				else
				{
					Vector2 vector4 = this.Nodes[j + 1] - this.Nodes[j + 2];
					curve.EndTangent = curve.End + (vector4.normalized + -a.normalized).normalized * curve.Length * 0.333333343f;
				}
			}
		}
		else
		{
			for (int k = 0; k < num; k++)
			{
				Vector2 vector5 = this.Nodes[k];
				Vector2 vector6 = this.Nodes[this.GetIndex(k + 1)];
				Vector2 vector7 = vector6 - vector5;
				Spline2.Curve curve2 = this.curves[k];
				curve2.Length = vector7.magnitude;
				curve2.Start = vector5;
				curve2.End = vector6;
				this.fullLength += curve2.Length;
				Vector2 vector8 = this.Nodes[k] - this.Nodes[this.GetIndex(k - 1)];
				curve2.StartTangent = curve2.Start + (vector8.normalized + vector7.normalized).normalized * curve2.Length * 0.333333343f;
				Vector2 vector9 = this.Nodes[this.GetIndex(k + 1)] - this.Nodes[this.GetIndex(k + 2)];
				curve2.EndTangent = curve2.End + (vector9.normalized + -vector7.normalized).normalized * curve2.Length * 0.333333343f;
			}
		}
		this.CalculateRealLength(60);
	}

	private Spline2.Curve FindCurve(float position, out float amount, out int curveIndex)
	{
		if (position < 0f)
		{
			amount = 0f;
			curveIndex = 0;
			return this.curves[0];
		}
		if (position >= this.fullLength)
		{
			Spline2.Curve result = this.curves[this.curves.Count - 1];
			amount = 1f;
			curveIndex = this.curves.Count - 1;
			return result;
		}
		amount = 0f;
		float num = 0f;
		for (int i = 0; i < this.curves.Count; i++)
		{
			float length = this.curves[i].Length;
			if (position < num + length)
			{
				amount = (position - num) / length;
				curveIndex = i;
				return this.curves[i];
			}
			num += length;
		}
		curveIndex = 0;
		return this.curves[0];
	}

	public Vector2 Evaluate(float position)
	{
		this.CalculateSpline();
		if (this.curves == null || this.curves.Count < 1)
		{
			return Vector2.zero;
		}
		float mu;
		int num;
		Spline2.Curve curve = this.FindCurve(position, out mu, out num);
		return new Vector2(this.Inter(mu, curve.Start.x, curve.End.x, curve.StartTangent.x, curve.EndTangent.x), this.Inter(mu, curve.Start.y, curve.End.y, curve.StartTangent.y, curve.EndTangent.y));
	}

	public void EvaluateTwoPositions(float position, out Vector2 pos1, out Vector2 pos2)
	{
		this.CalculateSpline();
		float num;
		int num2;
		Spline2.Curve curve = this.FindCurve(position, out num, out num2);
		pos1 = new Vector2(this.Inter(num, curve.Start.x, curve.End.x, curve.StartTangent.x, curve.EndTangent.x), this.Inter(num, curve.Start.y, curve.End.y, curve.StartTangent.y, curve.EndTangent.y));
		num += 0.001f;
		pos2 = new Vector2(this.Inter(num, curve.Start.x, curve.End.x, curve.StartTangent.x, curve.EndTangent.x), this.Inter(num, curve.Start.y, curve.End.y, curve.StartTangent.y, curve.EndTangent.y));
	}

	public Spline2.SplineInfo EvaluateWithInfo(float position)
	{
		this.CalculateSpline();
		Spline2.SplineInfo result = default(Spline2.SplineInfo);
		if (this.curves == null || this.curves.Count < 1)
		{
			return result;
		}
		float num;
		int curveIndex;
		Spline2.Curve curve = this.FindCurve(position, out num, out curveIndex);
		result.alpha = num;
		result.curve = curve;
		result.curveIndex = curveIndex;
		result.position = new Vector2(this.Inter(num, curve.Start.x, curve.End.x, curve.StartTangent.x, curve.EndTangent.x), this.Inter(num, curve.Start.y, curve.End.y, curve.StartTangent.y, curve.EndTangent.y));
		return result;
	}

	private float Inter(float mu, float x0, float x1, float tangent0, float tangent1)
	{
		return x0 * (1f - mu) * (1f - mu) * (1f - mu) + 3f * tangent0 * ((1f - mu) * (1f - mu)) * mu + 3f * tangent1 * (1f - mu) * mu * mu + x1 * (mu * mu * mu);
	}

	private void CalculateRealLength(int precision)
	{
		Vector2 a = this.Evaluate(0f);
		float num = this.fullLength / (float)(precision - 1);
		this.realLength = 0f;
		for (int i = 0; i < precision; i++)
		{
			float num2 = (float)i * num;
			Vector2 vector = this.Evaluate(num2 + num);
			this.realLength += (a - vector).magnitude;
			a = vector;
		}
	}

	public List<Vector2> Nodes = new List<Vector2>();

	public bool Cyclic;

	private bool isDirty = true;

	[SerializeField]
	public List<Spline2.Curve> curves;

	[SerializeField]
	private float fullLength;

	private float realLength;

	private List<Spline2.Segment> segments;

	private float segmentsLength;

	public delegate void DirtyHandler(Spline2 spline);

	[Serializable]
	public class Curve
	{
		[SerializeField]
		public Vector2 Start;

		[SerializeField]
		public Vector2 StartTangent;

		[SerializeField]
		public Vector2 End;

		[SerializeField]
		public Vector2 EndTangent;

		[SerializeField]
		public float Length;
	}

	private class Segment
	{
		public Segment(Vector2 from, Vector2 to, float start, float end)
		{
			this.from = from;
			this.to = to;
			this.length = (from - to).magnitude;
			this.start = start;
			this.end = end;
		}

		public Vector2 from;

		public Vector2 to;

		public float start;

		public float end;

		public float length;
	}

	public struct SplineInfo
	{
		public Vector2 position;

		public Spline2.Curve curve;

		public int curveIndex;

		public float alpha;
	}
}
