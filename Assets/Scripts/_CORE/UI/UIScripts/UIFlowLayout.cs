using System;
using System.Collections.Generic;
using UnityEngine;

public class UIFlowLayout : UILayout
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
		float num = -vector.x + this.padding.x;
		float num2 = vector.y;
		float num3 = 0f;
		int num4 = 0;
		int num5 = 0;
		int num6 = 100;
		while (num5 < childElements.Count && num6-- > 0)
		{
			bool flag = num == -vector.x + this.padding.x;
			bool flag2 = num + childElements[num5].Size.x > vector.x && !flag;
			if (!flag2)
			{
				childElements[num5].LocalPosition = new Vector2(num + childElements[num5].Size.x * 0.5f, num2 + childElements[num5].Size.y * -0.5f);
				num3 = Mathf.Max(num3, childElements[num5].Size.y);
				num += childElements[num5].Size.x + this.padding.x;
			}
			if (flag2 || num5 == childElements.Count - 1)
			{
				Vector2 a = Vector2.zero;
				if (this.center)
				{
					float num7 = num - -vector.x;
					a = Vector2.right * (elementSize.x - num7) * 0.5f;
				}
				if (this.centerY)
				{
					a -= Vector2.up * (elementSize.y - num3) * 0.5f;
				}
				for (int i = num4; i <= num5; i++)
				{
					Vector2 vector2 = Vector2.zero;
					if (this.centerY)
					{
						vector2 += Vector2.up * (childElements[i].Size.y - num3) * 0.5f;
					}
					childElements[i].LocalPosition += a + vector2;
				}
				if (flag2)
				{
					num4 = num5;
					num = -vector.x;
					num2 -= num3 + this.padding.y;
					continue;
				}
			}
			num5++;
		}
	}

	public bool center;

	public bool centerY;

	public Vector2 padding;
}
