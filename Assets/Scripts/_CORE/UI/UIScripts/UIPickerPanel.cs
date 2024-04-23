using System;
using UnityEngine;

public class UIPickerPanel : UIScrollablePanel
{
	public int SnapIndex
	{
		get
		{
			return this.currentSnapIndex;
		}
		set
		{
			this.currentSnapIndex = Mathf.Clamp(value, 0, Mathf.RoundToInt(base.TotalContentSize.x / this.snapSize) - 1);
		}
	}

	public void SetSnapIndex(int newIndex, bool animation = false)
	{
		this.currentSnapIndex = Mathf.Clamp(newIndex, 0, Mathf.RoundToInt(base.TotalContentSize.x / this.snapSize) - 1);
		this.snapOffsetTarget = this.IndexToOffset();
		if (!animation)
		{
			Vector2 actualScrollOffset = base.actualScrollOffset;
			actualScrollOffset.x = this.snapOffsetTarget;
			base.actualScrollOffset = actualScrollOffset;
			base.UpdateCellVisibility(false);
		}
	}

	private void UpdateSnapIndex()
	{
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal && this.snapSize > 0f)
		{
			int value = Mathf.FloorToInt(-base.actualScrollOffset.x / this.snapSize);
			this.currentSnapIndex = Mathf.Clamp(value, 0, Mathf.RoundToInt(base.TotalContentSize.x / this.snapSize) - 1);
		}
	}

	private float IndexToOffset()
	{
		return ((float)this.currentSnapIndex + 0.5f) * -this.snapSize;
	}

	protected override void ScrollLogic(ref Vector2 scrollOffset, float? verticalEdge, float? horizontalEdge)
	{
		int num = this.currentSnapIndex;
		base.ScrollLogic(ref scrollOffset, verticalEdge, horizontalEdge);
		this.UpdateSnapIndex();
		if (Mathf.Abs(this.speed.x) < 10f)
		{
			if (base.IsDragging)
			{
				this.snapOffsetTarget = this.IndexToOffset();
			}
			else if (this.snapSize > 0f)
			{
				if (Mathf.Abs(this.speed.x) > 0f)
				{
					this.snapOffsetTarget = this.IndexToOffset();
				}
				if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
				{
					float num2 = this.snapOffsetTarget - scrollOffset.x;
					this.speed.x = 0f;
					if (Mathf.Abs(num2) > base.Size.x * 0.02f)
					{
						scrollOffset.x += num2 * Time.deltaTime * (float)this.snapSpeed;
					}
					else
					{
						scrollOffset.x = this.snapOffsetTarget;
					}
				}
			}
		}
		if (num != this.currentSnapIndex && this.IndexChanged != null)
		{
			this.IndexChanged(this.currentSnapIndex);
		}
	}

	protected override void OnPress(bool pressed)
	{
		if (pressed && this.speed.magnitude > 0.01f)
		{
			this.UpdateSnapIndex();
			this.snapOffsetTarget = this.IndexToOffset();
		}
		base.OnPress(pressed);
	}

	public void BeginAdding()
	{
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Both)
		{
			this.scrollAxis = UIScrollablePanel.ScrollAxis.Vertical;
		}
		base.DestroyAllContent();
		this.elapsed = Vector3.zero;
	}

	public void AddToContent(UIElement e)
	{
		e.transform.parent = base.ScrollRoot;
		e.transform.localPosition = new Vector3(e.Size.x * 0.5f, e.Size.y * 0.5f, this.cellZPosition) + this.elapsed;
		e.autoSizing = (UIAutoSizing.LeftAnchor | UIAutoSizing.RightAnchor);
		e.gameObject.SetLayerRecursively(base.gameObject.layer);
		if (this.scrollAxis == UIScrollablePanel.ScrollAxis.Horizontal)
		{
			e.SetSizeAndDoLayout(new Vector2(e.Size.x, base.Size.y));
			this.elapsed.x = this.elapsed.x + (e.Size.x + this.paddingAmount);
		}
		else
		{
			e.SetSizeAndDoLayout(new Vector2(base.Size.x, e.Size.y));
			if (this.startFromTop)
			{
				this.elapsed.y = this.elapsed.y - (e.Size.y + this.paddingAmount);
			}
			else
			{
				this.elapsed.y = this.elapsed.y + (e.Size.y + this.paddingAmount);
			}
		}
	}

	public void EndAdding()
	{
		base.CalculateContentSize();
		if (this.startFromTop)
		{
			base.SetScroll(-base.TotalContentSize);
		}
		else
		{
			base.SetScroll(Vector2.zero);
		}
	}

	protected override Rect GetOffsetBoundaries()
	{
		return new Rect(this.snapSize * 0.5f, 0f, this.snapSize, 0f);
	}

	public int snapSpeed = 20;

	public float snapSize;

	public float cellZPosition;

	public float paddingAmount;

	private float snapOffsetTarget;

	private int currentSnapIndex;

	private Vector3 elapsed;

	public Action<int> IndexChanged;
}
