using System;
using System.Collections.Generic;
using UnityEngine;

public class UIVerticalLayout : UILayout
{
	public override void Layout()
	{
		List<UIElement> childElements = this.GetChildElements(UILayout.QueryFlags.SortByName | UILayout.QueryFlags.NoDisabled);
		if (childElements.Count == 0)
		{
			return;
		}
		Vector2 elementSize = this.GetElementSize();
		Vector2 vector = elementSize * 0.5f;
		float num = 0f;
		int num2 = 0;
		int num3 = 100;
		while (num2 < childElements.Count && num3-- > 0)
		{
			if (num2 > 0)
			{
				num += this.spacing;
			}
			float num4 = vector.y - num;
			float y = childElements[num2].Size.y;
			childElements[num2].LocalPosition = new Vector2(0f, num4 + y * -0.5f);
			num += y;
			num2++;
		}
		Vector2 zero = Vector2.zero;
		if (this.alignment == VAlignment.Center)
		{
			zero.y = (num - elementSize.y) * 0.5f;
		}
		else if (this.alignment == VAlignment.Bottom)
		{
			zero.y = num - elementSize.y;
		}
		foreach (UIElement uielement in childElements)
		{
			Vector2 size = uielement.Size;
			size.x = elementSize.x;
			uielement.LocalPosition += zero;
			uielement.SetSizeAndDoLayout(size);
		}
	}

	public VAlignment alignment;

	public float spacing;
}
