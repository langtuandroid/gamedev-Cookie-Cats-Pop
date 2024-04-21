using System;
using System.Collections.Generic;
using UnityEngine;

public class TopologyHelper
{
	public TopologyHelper()
	{
		this.tiles = new HashSet<Tile>();
	}

	public void AddCircle(Tile center, int radius)
	{
		List<Tile> list = new List<Tile>();
		list.Add(center);
		this.AddTile(center);
		while (radius > 0)
		{
			List<Tile> list2 = new List<Tile>(list);
			list.Clear();
			foreach (Tile tile in list2)
			{
				foreach (Tile tile2 in tile.GetNeighbours())
				{
					if (!(tile2 == Tile.Invalid))
					{
						if (!this.tiles.Contains(tile2))
						{
							list.Add(tile2);
							this.AddTile(tile2);
						}
					}
				}
			}
			radius--;
		}
	}

	public void AddTile(Tile tile)
	{
		if (tile == Tile.Invalid)
		{
			return;
		}
		this.tiles.Add(tile);
	}

	public void AddRay(Tile origin, Direction direction, int distance = 0)
	{
		Tile tile = origin;
		int num = 1;
		while (tile != Tile.Invalid)
		{
			this.AddTile(tile);
			if (distance > 0 && num >= distance)
			{
				break;
			}
			num++;
			tile = tile.GetNeighbour(direction);
		}
	}

	public void AddRow(Tile origin, int distance)
	{
		this.AddRay(origin, BubbleTopology.Left, distance);
		this.AddRay(origin, BubbleTopology.Right, distance);
	}

	public void AddRow(Tile origin, int height, int distance)
	{
		this.AddRow(origin, distance);
		Tile tile = origin;
		Tile tile2 = origin;
		for (int i = 1; i < height; i++)
		{
			if (i % 2 == 0)
			{
				if (tile2 != Tile.Invalid)
				{
					Tile neighbour = tile2.GetNeighbour(BubbleTopology.LeftDown);
					if (neighbour == Tile.Invalid)
					{
						neighbour = tile2.GetNeighbour(BubbleTopology.RightDown);
					}
					tile2 = neighbour;
					this.AddRow(tile2, distance);
				}
			}
			else if (tile != Tile.Invalid)
			{
				Tile neighbour2 = tile.GetNeighbour(BubbleTopology.LeftUp);
				if (neighbour2 == Tile.Invalid)
				{
					neighbour2 = tile.GetNeighbour(BubbleTopology.RightUp);
				}
				tile = neighbour2;
				this.AddRow(tile, distance);
			}
		}
	}

	public void AddColumn(Tile origin, int height)
	{
		Tile tile = origin;
		bool flag = false;
		int num = 0;
		while (!(tile == Tile.Invalid))
		{
			if (!flag)
			{
				this.AddTile(tile);
			}
			else
			{
				Tile neighbour = tile.GetNeighbour(BubbleTopology.LeftUp);
				Tile neighbour2 = tile.GetNeighbour(BubbleTopology.RightUp);
				bool flag2 = false;
				if (neighbour != Tile.Invalid)
				{
					this.AddTile(neighbour);
					flag2 = true;
					tile = neighbour.GetNeighbour(BubbleTopology.RightUp);
				}
				if (neighbour2 != Tile.Invalid)
				{
					this.AddTile(neighbour2);
					if (!flag2)
					{
						flag2 = true;
						tile = neighbour2.GetNeighbour(BubbleTopology.LeftUp);
					}
				}
				if (!flag2)
				{
					break;
				}
			}
			flag = !flag;
			num++;
			if (num <= height)
			{
				continue;
			}
			return;
		}
	}

	public void AddColumn(Tile origin, int width, int height)
	{
		this.AddColumn(origin, height);
		Tile tile = origin;
		Tile tile2 = origin;
		for (int i = 1; i < width; i++)
		{
			if (i % 2 == 0)
			{
				if (!(tile == Tile.Invalid))
				{
					tile = tile.GetNeighbour(BubbleTopology.Left);
					this.AddColumn(tile, height);
				}
			}
			else if (!(tile2 == Tile.Invalid))
			{
				tile2 = tile2.GetNeighbour(BubbleTopology.Right);
				this.AddColumn(tile2, height);
			}
		}
	}

	public List<Tile> GetSortedTiles(Comparison<Tile> sortFunction)
	{
		List<Tile> list = new List<Tile>(this.tiles);
		list.Sort(sortFunction);
		return list;
	}

	public Tile GetLeftMostTile(float yPosition)
	{
		List<Tile> sortedTiles = this.GetSortedTiles(delegate(Tile a, Tile b)
		{
			Vector3 localPosition = a.LocalPosition;
			Vector3 localPosition2 = b.LocalPosition;
			if (Mathf.Approximately(localPosition.x, localPosition2.x))
			{
				float num = Mathf.Abs(yPosition - localPosition.y);
				float value = Mathf.Abs(yPosition - localPosition2.y);
				return num.CompareTo(value);
			}
			return localPosition.x.CompareTo(localPosition2.x);
		});
		return (sortedTiles.Count <= 0) ? Tile.Invalid : sortedTiles[0];
	}

	public Tile GetRightMostTile(float yPosition)
	{
		List<Tile> sortedTiles = this.GetSortedTiles(delegate(Tile a, Tile b)
		{
			Vector3 localPosition = a.LocalPosition;
			Vector3 localPosition2 = b.LocalPosition;
			if (Mathf.Approximately(localPosition.x, localPosition2.x))
			{
				float num = Mathf.Abs(yPosition - localPosition.y);
				float value = Mathf.Abs(yPosition - localPosition2.y);
				return num.CompareTo(value);
			}
			return localPosition2.x.CompareTo(localPosition.x);
		});
		return (sortedTiles.Count <= 0) ? Tile.Invalid : sortedTiles[0];
	}

	public Tile GetBottomMostTile()
	{
		List<Tile> sortedTiles = this.GetSortedTiles((Tile a, Tile b) => a.LocalPosition.y.CompareTo(b.LocalPosition.y));
		return (sortedTiles.Count <= 0) ? Tile.Invalid : sortedTiles[0];
	}

	public float Distance(Tile a, Tile b)
	{
		return Vector3.Magnitude(a.LocalPosition - b.LocalPosition);
	}

	public IEnumerable<TopologyHelper.DistanceTile> GetDistanceTiles(Tile origin)
	{
		return this.GetDistanceTiles(origin, new Func<Tile, Tile, float>(this.Distance));
	}

	public IEnumerable<TopologyHelper.DistanceTile> GetDistanceTiles(Tile origin, Func<Tile, Tile, float> distanceFunction)
	{
		foreach (Tile item in this.tiles)
		{
			yield return new TopologyHelper.DistanceTile
			{
				tile = item,
				distance = distanceFunction(origin, item)
			};
		}
		yield break;
	}

	public IEnumerable<Tile> GetTiles()
	{
		foreach (Tile tile in this.tiles)
		{
			yield return tile;
		}
		yield break;
	}

	public IEnumerable<List<Tile>> GetRowTiles()
	{
		Dictionary<int, List<Tile>> rowTiles = new Dictionary<int, List<Tile>>();
		foreach (Tile item2 in this.tiles)
		{
			List<Tile> list;
			if (!rowTiles.TryGetValue(item2.Coord.y, out list))
			{
				list = new List<Tile>();
				rowTiles.Add(item2.Coord.y, list);
			}
			list.Add(item2);
		}
		foreach (KeyValuePair<int, List<Tile>> item in rowTiles)
		{
			yield return item.Value;
		}
		yield break;
	}

	private HashSet<Tile> tiles;

	public class DistanceTile
	{
		public Tile tile;

		public float distance;
	}
}
