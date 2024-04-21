using System;
using System.Collections.Generic;
using UnityEngine;

public interface ITopology
{
	int GetIndexFromCoord(Coord c);

	Coord GetCoordFromIndex(int index);

	int GetNeighbourIndex(int originIndex, Direction dir);

	Vector2 GetPositionFromTile(int index);

	int Count { get; }

	Direction ApproximateFromAngle(float degree);

	IEnumerable<Direction> AllDirections();
}
