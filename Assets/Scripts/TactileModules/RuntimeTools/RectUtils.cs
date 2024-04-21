using System;
using UnityEngine;

namespace TactileModules.RuntimeTools
{
	public static class RectUtils
	{
		public static Rect GetBounds(params Rect[] rects)
		{
			if (rects.Length == 0)
			{
				return Rect.zero;
			}
			Rect result = rects[0];
			for (int i = 1; i < rects.Length; i++)
			{
				Rect rect = rects[i];
				result.xMin = Mathf.Min(rect.xMin, result.xMin);
				result.yMin = Mathf.Min(rect.yMin, result.yMin);
				result.xMax = Mathf.Max(rect.xMax, result.xMax);
				result.yMax = Mathf.Max(rect.yMax, result.yMax);
			}
			return result;
		}
	}
}
