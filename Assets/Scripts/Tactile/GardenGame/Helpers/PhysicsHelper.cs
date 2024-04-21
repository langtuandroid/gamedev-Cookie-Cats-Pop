using System;
using UnityEngine;

namespace Tactile.GardenGame.Helpers
{
	public static class PhysicsHelper
	{
		public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{
			Vector3 lhs = linePoint2 - linePoint1;
			Vector3 rhs = Vector3.Cross(lineVec1, lineVec2);
			Vector3 lhs2 = Vector3.Cross(lhs, lineVec2);
			float f = Vector3.Dot(lhs, rhs);
			if (Mathf.Abs(f) < 0.0001f && rhs.sqrMagnitude > 0.0001f)
			{
				float d = Vector3.Dot(lhs2, rhs) / rhs.sqrMagnitude;
				intersection = linePoint1 + lineVec1 * d;
				return true;
			}
			intersection = Vector3.zero;
			return false;
		}

		public static bool FastLineSegmentIntersectionTest(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
		{
			Vector2 vector = p2 - p1;
			Vector2 vector2 = p3 - p4;
			Vector2 vector3 = p1 - p3;
			float num = vector2.y * vector3.x - vector2.x * vector3.y;
			float num2 = vector.y * vector2.x - vector.x * vector2.y;
			float num3 = vector.x * vector3.y - vector.y * vector3.x;
			float num4 = vector.y * vector2.x - vector.x * vector2.y;
			bool flag = true;
			if (num2 == 0f || num4 == 0f)
			{
				flag = false;
			}
			else
			{
				if (num2 > 0f)
				{
					if (num < 0f || num > num2)
					{
						flag = false;
					}
				}
				else if (num > 0f || num < num2)
				{
					flag = false;
				}
				if (flag && num4 > 0f)
				{
					if (num3 < 0f || num3 > num4)
					{
						flag = false;
					}
				}
				else if (num3 > 0f || num3 < num4)
				{
					flag = false;
				}
			}
			return flag;
		}
	}
}
