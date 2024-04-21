using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tactile.GardenGame.Helpers
{
	public static class IsometricUtils
	{
		public static float CalculateBearing(Vector2 dir)
		{
			dir = dir.normalized;
			float num;
			if (dir.x < 0f)
			{
				num = 360f - Mathf.Atan2(dir.x, dir.y) * 57.29578f * -1f;
			}
			else
			{
				num = Mathf.Atan2(dir.x, dir.y) * 57.29578f;
			}
			while (num >= 360f)
			{
				num -= 360f;
			}
			return num;
		}

		public static CardinalDirection GetNearestIsoCardinalDirection(Vector2 dir)
		{
			float bearing = IsometricUtils.CalculateBearing(dir);
			return IsometricUtils.GetNearestIsoCardinalDirection(bearing);
		}

		public static CardinalDirection GetNearestIsoCardinalDirection(float bearing)
		{
			CardinalDirection result = CardinalDirection.N;
			float num = float.MaxValue;
			for (CardinalDirection cardinalDirection = CardinalDirection.N; cardinalDirection < CardinalDirection.NONE; cardinalDirection++)
			{
				float num2 = Mathf.Abs(IsometricUtils.GetIsoBearing(cardinalDirection) - bearing);
				if (num2 < num)
				{
					num = num2;
					result = cardinalDirection;
				}
			}
			float num3 = Mathf.Abs(360f - bearing);
			if (num3 < num)
			{
				result = CardinalDirection.N;
			}
			return result;
		}

		public static float GetIsoBearing(CardinalDirection dir)
		{
			switch (dir)
			{
			case CardinalDirection.N:
				return 0f;
			case CardinalDirection.NE:
				return 63f;
			case CardinalDirection.E:
				return 90f;
			case CardinalDirection.SE:
				return 117f;
			case CardinalDirection.S:
				return 180f;
			case CardinalDirection.SW:
				return 243f;
			case CardinalDirection.W:
				return 270f;
			case CardinalDirection.NW:
				return 297f;
			default:
				return 0f;
			}
		}

		public static Vector2 GetISODirection(CardinalDirection dir)
		{
			float isoBearing = IsometricUtils.GetIsoBearing(dir);
			return IsometricUtils.GetDirectionFromBearing(isoBearing);
		}

		public static Vector2 GetDirectionFromBearing(float bearing)
		{
			bearing *= 0.0174532924f;
			Vector2 result = new Vector2(Mathf.Sin(bearing), Mathf.Cos(bearing));
			return result;
		}

		public static bool IsDirectionCardinal(Vector2 direction, float threshold)
		{
			float num = IsometricUtils.CalculateBearing(direction);
			CardinalDirection nearestIsoCardinalDirection = IsometricUtils.GetNearestIsoCardinalDirection(num);
			float isoBearing = IsometricUtils.GetIsoBearing(nearestIsoCardinalDirection);
			float num2 = Mathf.Abs(isoBearing - num);
			if (num2 >= 360f)
			{
				num2 -= 360f;
			}
			return num2 <= threshold;
		}

		public static List<Vector2> GetCardinalOnlyPath(Vector2 startPos, Vector2 endPos, float threshold)
		{
			List<Vector2> list = new List<Vector2>();
			list.Add(startPos);
			Vector2 vector = endPos - startPos;
			if (!IsometricUtils.IsDirectionCardinal(vector, threshold))
			{
				float magnitude = vector.magnitude;
				CardinalDirection nearestIsoCardinalDirection = IsometricUtils.GetNearestIsoCardinalDirection(vector);
				Vector2 isodirection = IsometricUtils.GetISODirection(nearestIsoCardinalDirection);
				for (CardinalDirection cardinalDirection = CardinalDirection.N; cardinalDirection < CardinalDirection.NONE; cardinalDirection++)
				{
					if (cardinalDirection != nearestIsoCardinalDirection)
					{
						Vector2 isodirection2 = IsometricUtils.GetISODirection(cardinalDirection);
						Vector2 vector2 = endPos - isodirection2 * magnitude;
						if (PhysicsHelper.FastLineSegmentIntersectionTest(startPos, startPos + isodirection * magnitude, endPos, vector2))
						{
							Vector3 v;
							PhysicsHelper.LineLineIntersection(out v, startPos, isodirection, endPos, endPos - vector2);
							list.Add(v);
							break;
						}
					}
				}
			}
			list.Add(endPos);
			return list;
		}
	}
}
