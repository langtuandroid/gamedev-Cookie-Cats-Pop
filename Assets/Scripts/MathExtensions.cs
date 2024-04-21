using System;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
	public static float DirectionToDegrees(this Vector2 dir)
	{
		float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	public static float Average(this List<int> numbers)
	{
		float num = 0f;
		foreach (int num2 in numbers)
		{
			num += (float)num2;
		}
		return num / (float)numbers.Count;
	}

	public static float Average(this List<float> numbers)
	{
		float num = 0f;
		foreach (float num2 in numbers)
		{
			float num3 = num2;
			num += num3;
		}
		return num / (float)numbers.Count;
	}

	public static float Average(this Queue<float> numbers)
	{
		float num = 0f;
		foreach (float num2 in numbers)
		{
			float num3 = num2;
			num += num3;
		}
		return num / (float)numbers.Count;
	}
}
