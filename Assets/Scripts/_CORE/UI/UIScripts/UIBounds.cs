using System;
using UnityEngine;

public struct UIBounds
{
	public Vector2 Size
	{
		get
		{
			return this.max - this.min;
		}
	}

	public Vector2 Center
	{
		get
		{
			return this.min + this.Size * 0.5f;
		}
	}

	public void Encapsulate(Vector2 p)
	{
		this.min.x = Mathf.Min(this.min.x, p.x);
		this.min.y = Mathf.Min(this.min.y, p.y);
		this.max.x = Mathf.Max(this.max.x, p.x);
		this.max.y = Mathf.Max(this.max.y, p.y);
	}

	public void Encapsulate(Rect r)
	{
		this.min.x = Mathf.Min(this.min.x, r.min.x);
		this.min.y = Mathf.Min(this.min.y, r.min.y);
		this.max.x = Mathf.Max(this.max.x, r.max.x);
		this.max.y = Mathf.Max(this.max.y, r.max.y);
	}

	public Vector2 min;

	public Vector2 max;

	public static readonly UIBounds Empty = new UIBounds
	{
		min = Vector2.one * float.MaxValue,
		max = Vector2.one * float.MinValue
	};
}
