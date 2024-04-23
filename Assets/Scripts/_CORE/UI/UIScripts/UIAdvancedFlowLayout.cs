using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAdvancedFlowLayout : UILayout
{
	public override void Layout()
	{
		List<UIElement> childElements = this.GetChildElements(UILayout.QueryFlags.NoDisabled);
		if (childElements.Count == 0)
		{
			return;
		}
		Vector2 elementSize = this.GetElementSize();
		Vector2 vector = elementSize * 0.5f;
		Vector4 margins = new Vector4
		{
			x = -vector.x + this.LayoutPadding.x,
			y = vector.y - this.LayoutPadding.y,
			z = vector.x - this.LayoutPadding.x,
			w = -vector.y + this.LayoutPadding.y
		};
		Vector2 cursor = this.CalculateStartingCursor(margins);
		Vector2 vector2 = default(Vector2);
		float num = 0f;
		Vector2 childSize = Vector2.one;
		int startIndex = 0;
		for (int i = 0; i < childElements.Count; i++)
		{
			UIElement child = childElements[i];
			childSize = childElements[i].Size;
			if (childSize.x > elementSize.x && vector2.x == 0f)
			{
				this.MoveChildToCursor(child, cursor);
				this.MoveCursorToNextRow(ref cursor, margins, childSize.y);
				num += childSize.y + this.VertialSpacing;
				vector2 = Vector2.zero;
				startIndex = i + 1;
			}
			else
			{
				if (this.WillOverflow(cursor, childSize, margins))
				{
					vector2.x -= this.HorizontalSpacing;
					if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
					{
						this.CenterRow(childElements, startIndex, vector2.x, elementSize.x - this.LayoutPadding.x * 2f, i - 1);
					}
					this.MoveCursorToNextRow(ref cursor, margins, vector2.y);
					num += vector2.y + this.VertialSpacing;
					vector2 = Vector2.zero;
					startIndex = i;
				}
				this.MoveChildToCursor(child, cursor);
				this.MoveCursorToNextCol(ref cursor, childSize);
				vector2.x += childSize.x + this.HorizontalSpacing;
				if (childSize.y > vector2.y)
				{
					vector2.y = childSize.y;
				}
			}
		}
		num += vector2.y;
		vector2.x -= this.HorizontalSpacing;
		if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			this.CenterRow(childElements, startIndex, vector2.x, elementSize.x - this.LayoutPadding.x * 2f, childElements.Count - 1);
		}
		if (this.VerticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Center)
		{
			this.CenterLayout(childElements, num, elementSize.y - this.LayoutPadding.y * 2f);
		}
	}

	private Vector2 CalculateStartingCursor(Vector4 margins)
	{
		Vector2 zero = Vector2.zero;
		UIAdvancedFlowLayout.HorizontalFlowAlignment horizontalAlignment = this.HorizontalAlignment;
		if (horizontalAlignment != UIAdvancedFlowLayout.HorizontalFlowAlignment.Left && horizontalAlignment != UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			if (horizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Right)
			{
				zero.x = margins.z;
			}
		}
		else
		{
			zero.x = margins.x;
		}
		UIAdvancedFlowLayout.VerticalFlowAlignment verticalAlignment = this.VerticalAlignment;
		if (verticalAlignment != UIAdvancedFlowLayout.VerticalFlowAlignment.Top && verticalAlignment != UIAdvancedFlowLayout.VerticalFlowAlignment.Center)
		{
			if (verticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Bottom)
			{
				zero.y = margins.w;
			}
		}
		else
		{
			zero.y = margins.y;
		}
		return zero;
	}

	private void MoveChildToCursor(UIElement child, Vector2 cursor)
	{
		Vector2 localPosition = cursor;
		if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Left || this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			localPosition.x += child.Size.x / 2f;
		}
		else
		{
			localPosition.x -= child.Size.x / 2f;
		}
		if (this.VerticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Top || this.VerticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Center)
		{
			localPosition.y -= child.Size.y / 2f;
		}
		else
		{
			localPosition.y += child.Size.y / 2f;
		}
		child.LocalPosition = localPosition;
	}

	private bool WillOverflow(Vector2 cursor, Vector2 childSize, Vector4 margins)
	{
		if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Left || this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			return cursor.x + childSize.x > margins.z;
		}
		return cursor.x - childSize.x < margins.x;
	}

	private void MoveCursorToNextRow(ref Vector2 cursor, Vector4 margins, float rowHeight)
	{
		if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Left)
		{
			cursor.x = margins.x;
		}
		else if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			cursor.x = margins.x;
		}
		else
		{
			cursor.x = margins.z;
		}
		if (this.VerticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Top || this.VerticalAlignment == UIAdvancedFlowLayout.VerticalFlowAlignment.Center)
		{
			cursor.y -= rowHeight + this.VertialSpacing;
		}
		else
		{
			cursor.y += rowHeight + this.VertialSpacing;
		}
	}

	private void MoveCursorToNextCol(ref Vector2 cursor, Vector2 childSize)
	{
		if (this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Left || this.HorizontalAlignment == UIAdvancedFlowLayout.HorizontalFlowAlignment.Center)
		{
			cursor.x += childSize.x + this.HorizontalSpacing;
		}
		else
		{
			cursor.x -= childSize.x + this.HorizontalSpacing;
		}
	}

	private void CenterRow(List<UIElement> children, int startIndex, float rowWidth, float layoutWidth, int lastIndex)
	{
		float x = (layoutWidth - rowWidth) / 2f;
		for (int i = startIndex; i <= lastIndex; i++)
		{
			UIElement uielement = children[i];
			uielement.LocalPosition += new Vector2(x, 0f);
		}
	}

	private void CenterLayout(List<UIElement> children, float contentHeight, float layoutHeight)
	{
		float y = (layoutHeight - contentHeight) / 2f;
		foreach (UIElement uielement in children)
		{
			uielement.LocalPosition -= new Vector2(0f, y);
		}
	}

	public UIAdvancedFlowLayout.HorizontalFlowAlignment HorizontalAlignment;

	public UIAdvancedFlowLayout.VerticalFlowAlignment VerticalAlignment;

	public float HorizontalSpacing;

	public float VertialSpacing;

	public Vector2 LayoutPadding;

	public enum HorizontalFlowAlignment
	{
		Left,
		Center,
		Right
	}

	public enum VerticalFlowAlignment
	{
		Top,
		Center,
		Bottom
	}
}
