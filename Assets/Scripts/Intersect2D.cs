using System;
using UnityEngine;

public static class Intersect2D
{
	public static bool ClosestCircleLineIntersection(Vector2 lineStart, Vector2 lineEnd, Vector2 circlePos, float radius, out Vector2 intersectPos)
	{
		float x = circlePos.x;
		float y = circlePos.y;
		intersectPos = Vector2.zero;
		Vector2 vector;
		Vector2 vector2;
		int num = Intersect2D.FindLineCircleIntersections(x, y, radius, lineStart, lineEnd, out vector, out vector2);
		if (num == 1)
		{
			intersectPos = vector;
			return true;
		}
		if (num == 2)
		{
			double num2 = Intersect2D.Distance(vector, lineStart);
			double num3 = Intersect2D.Distance(vector2, lineStart);
			if (num2 < num3)
			{
				intersectPos = vector;
			}
			else
			{
				intersectPos = vector2;
			}
			return true;
		}
		return false;
	}

	private static double Distance(Vector2 p1, Vector2 p2)
	{
		return (double)Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2f) + Mathf.Pow(p2.y - p1.y, 2f));
	}

	private static int FindLineCircleIntersections(float cx, float cy, float radius, Vector2 point1, Vector2 point2, out Vector2 intersection1, out Vector2 intersection2)
	{
		float num = point2.x - point1.x;
		float num2 = point2.y - point1.y;
		float num3 = num * num + num2 * num2;
		float num4 = 2f * (num * (point1.x - cx) + num2 * (point1.y - cy));
		float num5 = (point1.x - cx) * (point1.x - cx) + (point1.y - cy) * (point1.y - cy) - radius * radius;
		float num6 = num4 * num4 - 4f * num3 * num5;
		if ((double)num3 <= 1E-07 || num6 < 0f)
		{
			intersection1 = new Vector2(float.NaN, float.NaN);
			intersection2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}
		if (num6 == 0f)
		{
			float num7 = -num4 / (2f * num3);
			intersection1 = new Vector2(point1.x + num7 * num, point1.y + num7 * num2);
			intersection2 = new Vector2(float.NaN, float.NaN);
			return 1;
		}
		float num8 = (-num4 + Mathf.Sqrt(num6)) / (2f * num3);
		float num9 = (-num4 - Mathf.Sqrt(num6)) / (2f * num3);
		if (num8 >= 0f && num8 <= 1f)
		{
			intersection1 = new Vector2(point1.x + num8 * num, point1.y + num8 * num2);
			intersection2 = new Vector2(point1.x + num9 * num, point1.y + num9 * num2);
			return 2;
		}
		intersection1 = new Vector2(float.NaN, float.NaN);
		intersection2 = new Vector2(float.NaN, float.NaN);
		return 0;
	}

	public static bool LineSegmentsIntersect(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersectionPoint)
	{
		float num = a1.x - a0.x;
		float num2 = a1.y - a0.y;
		float num3 = b1.x - b0.x;
		float num4 = b1.y - b0.y;
		float num5 = (-num2 * (a0.x - b0.x) + num * (a0.y - b0.y)) / (-num3 * num2 + num * num4);
		float num6 = (num3 * (a0.y - b0.y) - num4 * (a0.x - b0.x)) / (-num3 * num2 + num * num4);
		if (num5 >= 0f && num5 <= 1f && num6 >= 0f && num6 <= 1f)
		{
			float x = a0.x + num6 * num;
			float y = a0.y + num6 * num2;
			intersectionPoint = new Vector2(x, y);
			return true;
		}
		intersectionPoint = Vector2.zero;
		return false;
	}

	public static bool MovingCircleToCircleIntersect(Vector2 aCenter, float aRadius, Vector2 bCenter, float bRadius, Vector2 moveVector, out Vector2 collisionPoint)
	{
		collisionPoint = Vector2.zero;
		double num = (double)(bCenter - aCenter).magnitude;
		double num2 = (double)(bRadius + aRadius);
		num -= num2;
		if ((double)moveVector.magnitude < num)
		{
			return false;
		}
		Vector2 normalized = moveVector.normalized;
		Vector2 rhs = bCenter - aCenter;
		double num3 = (double)Vector2.Dot(normalized, rhs);
		if (num3 <= 0.0)
		{
			return false;
		}
		double num4 = (double)rhs.magnitude;
		double num5 = num4 * num4 - num3 * num3;
		double num6 = num2 * num2;
		if (num5 >= num6)
		{
			return false;
		}
		double num7 = num6 - num5;
		if (num7 < 0.0)
		{
			return false;
		}
		double num8 = num3 - (double)Mathf.Sqrt((float)num7);
		double num9 = (double)moveVector.magnitude;
		if (num9 < num8)
		{
			return false;
		}
		collisionPoint = aCenter + moveVector.normalized * (float)num8;
		return true;
	}
}
