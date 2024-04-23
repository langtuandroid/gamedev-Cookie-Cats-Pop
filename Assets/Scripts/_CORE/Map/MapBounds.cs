using System;
using UnityEngine;

public class MapBounds : MonoBehaviour
{
	public Vector2 Size
	{
		get
		{
			return new Vector2(this.max.x - this.min.x, this.max.y - this.min.y);
		}
	}

	public Vector2 Center
	{
		get
		{
			return new Vector2((this.max.x + this.min.x) / 2f, (this.max.y + this.min.y) / 2f);
		}
	}

	public Vector2 max;

	public Vector2 min;
}
