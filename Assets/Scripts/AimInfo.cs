using System;
using System.Collections.Generic;
using UnityEngine;

public class AimInfo
{
	public AimInfo(float angleOffset)
	{
		this.collisionPoints = new List<Vector2>();
		this.angleOffset = angleOffset;
	}

	public bool IsValidForShot
	{
		get
		{
			return this.direction.y >= 0.2f;
		}
	}

	public void Calculate(Vector2 muzzlePosition, Vector2 target, GameBoard board)
	{
		this.direction = (target - muzzlePosition).normalized;
		float num = this.direction.DirectionToDegrees();
		this.direction = Quaternion.Euler(0f, 0f, num + this.angleOffset) * Vector3.right;
		this.aimOriginInBoardSpace = board.Root.InverseTransformPoint(muzzlePosition);
		Tile tile;
		this.collisionPoints = Trajectory.CalculateTrajectoryHits(board, this.aimOriginInBoardSpace, this.direction, out tile);
	}

	public bool TryFindIntersectionWithHorizontalLine(float lineYPosition, out Vector2 intersectionPoint)
	{
		int i = this.collisionPoints.Count - 1;
		while (i >= 0)
		{
			if (this.collisionPoints[i].y < lineYPosition)
			{
				if (i == this.collisionPoints.Count - 1)
				{
					intersectionPoint = Vector2.zero;
					return false;
				}
				Vector2 vector = this.collisionPoints[i];
				Vector2 a = this.collisionPoints[i + 1];
				Vector2 a2 = a - vector;
				a2.Normalize();
				float d = (lineYPosition - vector.y) / a2.y;
				intersectionPoint = vector + a2 * d;
				return true;
			}
			else
			{
				i--;
			}
		}
		if (this.collisionPoints.Count > 0)
		{
			Vector2 vector2 = this.aimOriginInBoardSpace;
			Vector2 a3 = this.collisionPoints[0];
			Vector2 a4 = a3 - vector2;
			a4.Normalize();
			float d2 = (lineYPosition - vector2.y) / a4.y;
			intersectionPoint = vector2 + a4 * d2;
			return true;
		}
		intersectionPoint = Vector2.zero;
		return false;
	}

	private readonly float angleOffset;

	public Vector2 direction;

	public List<Vector2> collisionPoints;

	public Vector2 aimOriginInBoardSpace;
}
