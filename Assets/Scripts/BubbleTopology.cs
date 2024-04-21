using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleTopology : ITopology
{
	public BubbleTopology(int numx, int numy, float tileRadius)
	{
		this.NumX = numx;
		this.NumY = numy;
		this.tileRadius = tileRadius;
	}

	public int NumX { get; private set; }

	public int NumY { get; private set; }

	public int NumTilesInTwoRows
	{
		get
		{
			return this.NumX * 2 - 1;
		}
	}

	public static BubbleTopology CreateFromTotal(int numx, int total, float tileRadius)
	{
		BubbleTopology bubbleTopology = new BubbleTopology(numx, 0, tileRadius);
		bubbleTopology.NumY = bubbleTopology.GetCoordFromIndex(total - 1).y + 1;
		return bubbleTopology;
	}

	private void GeneratePositions()
	{
		this.tilePositions = new List<Vector2>();
		float line_DIST_FACTOR = BubbleTopology.LINE_DIST_FACTOR;
		float num = 1f + BubbleTopology.LINE_DIST_FACTOR * (float)(this.NumY - 1);
		for (int i = 0; i < this.NumY; i++)
		{
			int num2 = this.NumX;
			float num3 = 0f;
			if (i % 2 != 0)
			{
				num3 = 0.5f;
				num2 = this.NumX - 1;
			}
			for (int j = 0; j < num2; j++)
			{
				Vector2 a = new Vector2(num3, num);
				this.tilePositions.Add(a * this.tileRadius);
				num3 += 1f;
			}
			num -= line_DIST_FACTOR;
		}
	}

	public Vector2 GetPositionFromTile(int index)
	{
		return this.GetPositionFromCoord(this.GetCoordFromIndex(index));
	}

	public int GetTileFromPosition(Vector2 pos)
	{
		Coord coordFromPosition = this.GetCoordFromPosition(pos);
		return this.GetIndexFromCoord(coordFromPosition);
	}

	public Vector2 GetPositionFromCoord(Coord c)
	{
		Vector2 result;
		result.x = (float)c.x;
		result.y = (float)c.y * -BubbleTopology.LINE_DIST_FACTOR;
		if (c.y % 2 != 0)
		{
			result.x += 0.5f;
		}
		return result;
	}

	public Coord GetCoordFromPosition(Vector2 pos)
	{
		Coord result;
		result.y = Mathf.RoundToInt(pos.y / -BubbleTopology.LINE_DIST_FACTOR);
		if (result.y % 2 != 0)
		{
			pos.x -= 0.5f;
		}
		result.x = Mathf.RoundToInt(pos.x);
		return result;
	}

	public float TotalHeight
	{
		get
		{
			return this.tileRadius * (1f + BubbleTopology.LINE_DIST_FACTOR * (float)(this.NumY - 1));
		}
	}

	public float TotalWidth
	{
		get
		{
			return this.tileRadius * (float)this.NumX;
		}
	}

	public Vector2 this[int indexer]
	{
		get
		{
			if (this.tilePositions == null)
			{
				this.GeneratePositions();
			}
			return this.tilePositions[indexer];
		}
	}

	public IEnumerable<Vector2> Positions
	{
		get
		{
			if (this.tilePositions == null)
			{
				this.GeneratePositions();
			}
			return this.tilePositions;
		}
	}

	public int GetClosest(Vector2 p, float radius)
	{
		if (this.tilePositions == null)
		{
			this.GeneratePositions();
		}
		int result = -1;
		float num = 9999f;
		int num2 = 0;
		foreach (Vector2 a in this.tilePositions)
		{
			float magnitude = (a - p).magnitude;
			if (magnitude < radius && magnitude < num)
			{
				num = magnitude;
				result = num2;
			}
			num2++;
		}
		return result;
	}

	public int Count
	{
		get
		{
			if (this.tilePositions == null)
			{
				this.GeneratePositions();
			}
			return this.tilePositions.Count;
		}
	}

	public Coord GetCoordFromIndex(int i)
	{
		int num = this.NumX * 2 - 1;
		int num2 = i % num;
		int num3 = i / num * 2;
		if (num2 >= this.NumX)
		{
			num2 -= this.NumX;
			num3++;
		}
		return new Coord(num2, num3);
	}

	public int GetNeighbourIndex(int originIndex, Direction dir)
	{
		Coord c = this.GetCoordFromIndex(originIndex);
		int num = dir.Value / 60;
		Coord c2 = BubbleTopology.NeighboorLookup[c.y % 2, num];
		c += c2;
		int num2 = (c.y % 2 != 0) ? (this.NumX - 1) : this.NumX;
		if (c.x >= 0 && c.x < num2 && c.y >= 0 && c.y < this.NumY)
		{
			return this.GetIndexFromCoord(c.x, c.y);
		}
		return -1;
	}

	public IEnumerable<int> AllValidNeighbours(int originIndex)
	{
		foreach (Direction dir in this.AllDirections())
		{
			int idx = this.GetNeighbourIndex(originIndex, dir);
			if (idx > -1)
			{
				yield return idx;
			}
		}
		yield break;
	}

	public int GetIndexFromCoord(Coord c)
	{
		return this.GetIndexFromCoord(c.x, c.y);
	}

	public int GetIndexFromCoord(int x, int y)
	{
		int num = y / 2;
		int num2 = y % 2;
		return num * (this.NumX * 2 - 1) + num2 * this.NumX + x;
	}

	public IEnumerable<Direction> AllDirections()
	{
		for (int i = 0; i < 6; i++)
		{
			yield return new Direction(i * 60);
		}
		yield break;
	}

	public Direction ApproximateFromAngle(float degrees)
	{
		float num = 360f;
		Direction result = new Direction(0);
		if (degrees < 0f)
		{
			degrees += 360f;
		}
		foreach (Direction direction in this.AllDirections())
		{
			float num2 = Mathf.Abs((float)direction.Value - degrees);
			if (num2 < num)
			{
				num = num2;
				result = direction;
			}
		}
		return result;
	}

	// Note: this type is marked as 'beforefieldinit'.
	static BubbleTopology()
	{
		Coord[,] array = new Coord[2, 6];
		array[0, 0] = new Coord(1, 0);
		array[0, 1] = new Coord(0, -1);
		array[0, 2] = new Coord(-1, -1);
		array[0, 3] = new Coord(-1, 0);
		array[0, 4] = new Coord(-1, 1);
		array[0, 5] = new Coord(0, 1);
		array[1, 0] = new Coord(1, 0);
		array[1, 1] = new Coord(1, -1);
		array[1, 2] = new Coord(0, -1);
		array[1, 3] = new Coord(-1, 0);
		array[1, 4] = new Coord(0, 1);
		array[1, 5] = new Coord(1, 1);
		BubbleTopology.NeighboorLookup = array;
	}

	public static readonly Direction Right = new Direction(0);

	public static readonly Direction RightUp = new Direction(60);

	public static readonly Direction LeftUp = new Direction(120);

	public static readonly Direction Left = new Direction(180);

	public static readonly Direction LeftDown = new Direction(240);

	public static readonly Direction RightDown = new Direction(300);

	public static readonly float LINE_DIST_FACTOR = Mathf.Sqrt(3f) * 0.5f;

	private float tileRadius;

	private List<Vector2> tilePositions;

	private static Coord[,] NeighboorLookup;
}
