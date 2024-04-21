using System;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtility
{
	public static List<Vector3> GetPositionsForGridLayout(Vector2 cellSize, int rows, int columns)
	{
		return UIUtility.GetPositionsForGridLayout(cellSize, rows, columns, Vector3.zero, true);
	}

	public static List<Vector3> GetPositionsForGridLayout(Vector2 cellSize, int rows, int columns, Vector3 center, bool centerAligned)
	{
		List<Vector3> list = new List<Vector3>();
		Vector2 a = (!centerAligned) ? new Vector2(0f, 0f) : new Vector2((float)(columns - 1) * -0.5f, (float)(rows - 1) * 0.5f);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				Vector2 vector = Vector2.Scale(a + new Vector2((float)j, (float)(-(float)i)), cellSize);
				list.Add(center + new Vector3(vector.x, vector.y));
			}
		}
		return list;
	}

	public static List<Rect> GetRectsForGridLayout(Vector3 center, Vector2 cellSize, int rows, int columns, bool centerAligned)
	{
		List<Rect> list = new List<Rect>();
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				Vector2 a = (!centerAligned) ? new Vector2(cellSize.x * 0f, cellSize.y * 0.5f) : new Vector2((float)(columns - 1) * -0.5f, (float)(rows - 1) * 0.5f);
				Vector2 vector = Vector2.Scale(a + new Vector2((float)j, (float)(-(float)i)), cellSize);
				list.Add(new Rect(center.x + vector.x - cellSize.x * 0.5f, center.y + vector.y - cellSize.y * 0.5f, cellSize.x, cellSize.y));
			}
		}
		return list;
	}

	public static List<Rect> GetPagedGridRects(int rowsInPage, int columnsInPage, Vector3 pageSize, int amount)
	{
		List<Rect> list = new List<Rect>();
		int num = rowsInPage * columnsInPage;
		Vector2 vector = new Vector2(pageSize.x / (float)columnsInPage, pageSize.y / (float)rowsInPage);
		for (int i = 0; i < amount; i++)
		{
			Vector2 vector2 = pageSize * 0.5f;
			int num2 = i / num;
			int num3 = i % num % columnsInPage;
			int num4 = i % num / columnsInPage;
			Vector2 vector3 = new Vector2((float)num3 * vector.x, (float)num4 * vector.y) + new Vector2((float)num2 * pageSize.x, 0f);
			Rect item = new Rect(vector3.x, vector3.y, vector.x, vector.y);
			item.y = pageSize.y - item.y - vector.y - vector2.y;
			item.x -= vector2.x;
			list.Add(item);
		}
		return list;
	}

	public static Vector2 AdjustHeightForScreenAspect(Vector2 size)
	{
		if (size.x <= 0f)
		{
			return size;
		}
		float num = (float)Screen.height / (float)Screen.width;
		float num2 = size.y / size.x;
		if (num != num2)
		{
			size.y = size.x * num;
		}
		return size;
	}

	public static Vector2 AdjustSizeForScreenAspect(Vector2 size)
	{
		if (size.x <= 0f)
		{
			return size;
		}
		float num = (float)Screen.height / (float)Screen.width;
		float num2 = size.y / size.x;
		if (num > num2)
		{
			size.y = size.x * num;
		}
		else if (num < num2)
		{
			size.x = size.y / num;
		}
		return size;
	}

	public static Vector2 AdjustWidthForScreenAspect(Vector2 size)
	{
		if (size.y <= 0f)
		{
			return size;
		}
		float num = (float)Screen.height / (float)Screen.width;
		float num2 = size.y / size.x;
		if (num != num2)
		{
			size.x = size.y / num;
		}
		return size;
	}

	public static Vector2 CorrectSizeToAspect(Vector2 currentSize, float wantedAspect, AspectCorrection correction)
	{
		Vector2 result = currentSize;
		float num = currentSize.Aspect();
		if (correction == AspectCorrection.Fit)
		{
			if (num < wantedAspect)
			{
				result.y = result.x / wantedAspect;
			}
			else
			{
				result.x = result.y * wantedAspect;
			}
		}
		else if (correction == AspectCorrection.Fill)
		{
			if (num < wantedAspect)
			{
				result.x = result.y * wantedAspect;
			}
			else
			{
				result.y = result.x / wantedAspect;
			}
		}
		return result;
	}

	public static float FindScaleFactorToFit(Vector2 size, Vector2 sizeToFitInside, AspectCorrection correction = AspectCorrection.Fit)
	{
		float a = sizeToFitInside.x / size.x;
		float b = sizeToFitInside.y / size.y;
		if (correction == AspectCorrection.Fit)
		{
			return Mathf.Min(a, b);
		}
		return Mathf.Max(a, b);
	}
}
