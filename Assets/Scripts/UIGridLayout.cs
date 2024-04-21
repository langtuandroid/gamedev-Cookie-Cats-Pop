using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGridLayout : UILayout
{
	public override void Layout()
	{
		List<UIElement> childElements = this.GetChildElements(UILayout.QueryFlags.SortByName | UILayout.QueryFlags.NoDisabled);
		Vector2 elementSize = this.GetElementSize();
		this.numColums = Mathf.Max(this.numColums, 1);
		int num = Mathf.Max(Mathf.CeilToInt((float)childElements.Count / (float)this.numColums), 1);
		if (this.centerCells)
		{
			float num2 = (this.maxElementSize.x <= 0f) ? (elementSize.y / (float)num) : this.maxElementSize.x;
			float num3 = (this.maxElementSize.y <= 0f) ? (elementSize.x / (float)this.numColums) : this.maxElementSize.y;
			float b = num2 * (float)this.numColums + this.centerCellsPadding * (float)(this.numColums - 1);
			float b2 = num3 * (float)num + this.centerCellsPadding * (float)(num - 1);
			elementSize.Set(Mathf.Min(elementSize.x, b), Mathf.Min(elementSize.y, b2));
		}
		Vector2 cellSize = new Vector2(elementSize.x / (float)this.numColums, elementSize.y / (float)num);
		List<Rect> rectsForGridLayout = UIUtility.GetRectsForGridLayout(Vector3.zero, cellSize, num, this.numColums, true);
		for (int i = 0; i < childElements.Count; i++)
		{
			if (i >= rectsForGridLayout.Count)
			{
				break;
			}
			Vector2 size = rectsForGridLayout[i].Size();
			if (this.maxElementSize.x > 0f)
			{
				size.x = Mathf.Min(size.x, this.maxElementSize.x);
			}
			if (this.maxElementSize.y > 0f)
			{
				size.y = Mathf.Min(size.y, this.maxElementSize.y);
			}
			childElements[i].Size = size;
			if (this.rightAlign)
			{
				childElements[i].LocalPosition = rectsForGridLayout[rectsForGridLayout.Count - (1 + i)].center;
			}
			else
			{
				childElements[i].LocalPosition = rectsForGridLayout[i].center;
			}
		}
	}

	public int numColums = 3;

	public Vector2 maxElementSize = Vector2.zero;

	public bool rightAlign;

	public bool centerCells;

	public float centerCellsPadding;
}
